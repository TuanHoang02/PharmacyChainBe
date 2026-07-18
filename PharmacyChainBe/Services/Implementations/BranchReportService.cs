using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class BranchReportService : IBranchReportService
    {
        private const int NearExpiryWindowDays = 90;

        private readonly AppDbContext _db;

        public BranchReportService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<BranchReportResponseDto> GetSalesReportAsync(int branchId, DateTime startDate, DateTime endDate)
        {
            var branch = await GetActiveBranchAsync(branchId);

            var startUtc = startDate.Date.ToUniversalTime();
            var endUtc = endDate.Date.AddDays(1).ToUniversalTime();

            var invoices = await _db.SalesInvoices.AsNoTracking()
                .Where(si => si.BranchID == branchId
                    && si.CreatedAt >= startUtc
                    && si.CreatedAt < endUtc
                    && si.PaymentStatus == PaymentStatus.Completed)
                .Include(si => si.SalesInvoiceDetails)
                    .ThenInclude(d => d.Medicine)
                .OrderByDescending(si => si.CreatedAt)
                .ToListAsync();

            if (invoices.Count == 0)
            {
                return new BranchReportResponseDto
                {
                    BranchName = branch.BranchName,
                    GeneratedAt = DateTime.UtcNow,
                    StartDate = startDate,
                    EndDate = endDate,
                    ReportType = "Sales",
                    Summary = new ReportSummaryDto(),
                    SalesDetails = new List<SalesReportDetailDto>(),
                };
            }

            var details = invoices.Select(si => new SalesReportDetailDto
            {
                InvoiceCode = si.InvoiceCode,
                InvoiceDate = si.CreatedAt,
                CustomerName = si.CustomerName,
                PaymentMethod = si.PaymentMethod.ToString(),
                PaymentStatus = si.PaymentStatus.ToString(),
                TotalItems = si.SalesInvoiceDetails.Sum(d => d.Quantity),
                Subtotal = si.Subtotal,
                DiscountAmount = si.DiscountAmount,
                TotalAmount = si.TotalAmount,
            }).ToList();

            var totalRevenue = invoices.Sum(si => si.TotalAmount);
            var totalItemsSold = invoices.Sum(si => si.SalesInvoiceDetails.Sum(d => d.Quantity));

            return new BranchReportResponseDto
            {
                BranchName = branch.BranchName,
                GeneratedAt = DateTime.UtcNow,
                StartDate = startDate,
                EndDate = endDate,
                ReportType = "Sales",
                Summary = new ReportSummaryDto
                {
                    TotalOrders = invoices.Count,
                    TotalRevenue = totalRevenue,
                    TotalItemsSold = totalItemsSold,
                    AverageOrderValue = invoices.Count > 0 ? totalRevenue / invoices.Count : 0m,
                },
                SalesDetails = details,
            };
        }

        public async Task<BranchReportResponseDto> GetInventoryReportAsync(int branchId)
        {
            var branch = await GetActiveBranchAsync(branchId);

            var now = DateTime.UtcNow;
            var nearExpiryCutoff = now.Date.AddDays(NearExpiryWindowDays);

            var inventories = await _db.Inventories.AsNoTracking()
                .Where(i => i.BranchID == branchId && i.QuantityInStock > 0)
                .Include(i => i.Medicine)
                .ToListAsync();

            if (inventories.Count == 0)
            {
                return new BranchReportResponseDto
                {
                    BranchName = branch.BranchName,
                    GeneratedAt = DateTime.UtcNow,
                    StartDate = now,
                    EndDate = now,
                    ReportType = "Inventory",
                    Summary = new ReportSummaryDto(),
                    InventoryDetails = new List<InventoryReportDetailDto>(),
                };
            }

            var medicineIds = inventories.Select(i => i.MedicineID).ToList();
            var batches = await _db.MedicineBatches.AsNoTracking()
                .Where(b => b.BranchID == branchId
                    && b.RemainingQuantity > 0
                    && medicineIds.Contains(b.MedicineID))
                .ToListAsync();

            var batchLookup = batches
                .GroupBy(b => b.MedicineID)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

            var details = inventories.Select(i =>
            {
                var stockStatus = GetStockStatus(i.QuantityInStock, i.ReorderLevel);
                var medBatches = batchLookup.GetValueOrDefault(i.MedicineID) ?? new List<MedicineBatch>();
                var nearestExpiry = medBatches
                    .Where(b => b.ExpiryDate > now)
                    .OrderBy(b => b.ExpiryDate)
                    .FirstOrDefault()?.ExpiryDate;

                return new InventoryReportDetailDto
                {
                    MedicineId = i.MedicineID,
                    MedicineName = i.Medicine?.MedicineName ?? string.Empty,
                    Unit = i.Medicine?.Unit,
                    QuantityInStock = i.QuantityInStock,
                    ReorderLevel = i.ReorderLevel,
                    SellingPrice = i.Medicine?.SellingPrice ?? 0m,
                    StockValue = i.QuantityInStock * (i.Medicine?.SellingPrice ?? 0m),
                    TotalBatches = medBatches.Count,
                    NearestExpiryDate = nearestExpiry,
                    StockStatus = stockStatus,
                };
            }).OrderByDescending(d => d.StockValue).ToList();

            var totalStockValue = details.Sum(d => d.StockValue);
            var lowStockCount = details.Count(d => d.StockStatus == "Low Stock");
            var nearExpiryCount = details.Count(d =>
                d.NearestExpiryDate.HasValue && d.NearestExpiryDate.Value <= nearExpiryCutoff);
            var expiredCount = batches.Count(b => b.ExpiryDate < now.Date);

            return new BranchReportResponseDto
            {
                BranchName = branch.BranchName,
                GeneratedAt = DateTime.UtcNow,
                StartDate = now,
                EndDate = now,
                ReportType = "Inventory",
                Summary = new ReportSummaryDto
                {
                    TotalSkus = details.Count,
                    TotalStockValue = totalStockValue,
                    LowStockCount = lowStockCount,
                    NearExpiryCount = nearExpiryCount,
                    ExpiredCount = expiredCount,
                },
                InventoryDetails = details,
            };
        }

        private async Task<Branch> GetActiveBranchAsync(int branchId)
        {
            var branch = await _db.Branches.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BranchID == branchId && b.IsActive);
            if (branch == null)
            {
                throw new ApiException("Không tìm thấy chi nhánh hoặc chi nhánh đã bị vô hiệu.", 404);
            }
            return branch;
        }

        private static string GetStockStatus(int quantity, int reorderLevel)
        {
            if (quantity <= 0) return "Out of Stock";
            if (quantity <= reorderLevel) return "Low Stock";
            if (quantity <= reorderLevel * 2) return "Medium Stock";
            return "In Stock";
        }
    }
}
