using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class BranchDashboardService : IBranchDashboardService
    {
        // ponytail: 90-day window for "near expiry". Magic ceiling named here per
        // project rule so the upgrade path (config-driven) is obvious later.
        private const int NearExpiryWindowDays = 90;

        private readonly AppDbContext _db;

        public BranchDashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<BranchDashboardDto> GetDashboardAsync(int branchId)
        {
            var branch = await _db.Branches.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BranchID == branchId && b.IsActive);
            if (branch == null)
            {
                throw new ApiException("Không tìm thấy chi nhánh hoặc chi nhánh đã bị vô hiệu.", 404);
            }

            var sales = await BuildSalesSummaryAsync(branchId);
            var inventory = await BuildInventoryStatusAsync(branchId);
            var lowStock = await BuildLowStockListAsync(branchId);
            var pendingRequests = await _db.PurchaseRequests.AsNoTracking()
                .CountAsync(pr => pr.BranchID == branchId
                    && pr.Status == PurchaseRequestStatus.Pending);

            return new BranchDashboardDto
            {
                BranchName = branch.BranchName,
                GeneratedAt = DateTime.UtcNow,
                Sales = sales,
                Inventory = inventory,
                LowStockMedicines = lowStock,
                PendingPurchaseOrders = pendingRequests,
            };
        }

        private async Task<SalesSummaryDto> BuildSalesSummaryAsync(int branchId)
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = todayStart.AddDays(-(int)todayStart.DayOfWeek);
            var monthStart = new DateTime(todayStart.Year, todayStart.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var branchInvoices = _db.SalesInvoices.AsNoTracking()
                .Where(si => si.BranchID == branchId);

            var paidInvoices = branchInvoices.Where(si => si.PaymentStatus == PaymentStatus.Completed);

            return new SalesSummaryDto
            {
                TodayRevenue = await paidInvoices
                    .Where(si => si.CreatedAt >= todayStart)
                    .SumAsync(si => (decimal?)si.TotalAmount) ?? 0m,
                WeekRevenue = await paidInvoices
                    .Where(si => si.CreatedAt >= weekStart)
                    .SumAsync(si => (decimal?)si.TotalAmount) ?? 0m,
                MonthRevenue = await paidInvoices
                    .Where(si => si.CreatedAt >= monthStart)
                    .SumAsync(si => (decimal?)si.TotalAmount) ?? 0m,
                TodayInvoices = await branchInvoices
                    .Where(si => si.CreatedAt >= todayStart)
                    .CountAsync(),
                PendingInvoices = await branchInvoices
                    .Where(si => si.PaymentStatus == PaymentStatus.Pending
                              || si.InvoiceStatus == InvoiceStatus.Draft)
                    .CountAsync(),
            };
        }

        private async Task<InventoryStatusDto> BuildInventoryStatusAsync(int branchId)
        {
            var now = DateTime.UtcNow;
            var nearExpiryCutoff = now.Date.AddDays(NearExpiryWindowDays);
            var today = now.Date;

            var totalSkus = await _db.Inventories.AsNoTracking()
                .Where(i => i.BranchID == branchId && i.QuantityInStock > 0)
                .Select(i => i.MedicineID)
                .Distinct()
                .CountAsync();

            var totalBatches = await _db.MedicineBatches.AsNoTracking()
                .CountAsync(mb => mb.BranchID == branchId && mb.RemainingQuantity > 0);

            var expiredBatches = await _db.MedicineBatches.AsNoTracking()
                .CountAsync(mb => mb.BranchID == branchId
                    && mb.RemainingQuantity > 0
                    && mb.ExpiryDate < today);

            var nearExpiryBatches = await _db.MedicineBatches.AsNoTracking()
                .CountAsync(mb => mb.BranchID == branchId
                    && mb.RemainingQuantity > 0
                    && mb.ExpiryDate >= today
                    && mb.ExpiryDate <= nearExpiryCutoff);

            var stockValue = await _db.Inventories.AsNoTracking()
                .Where(i => i.BranchID == branchId)
                .Join(_db.Medicines.AsNoTracking(),
                    i => i.MedicineID,
                    m => m.MedicineID,
                    (i, m) => new { i.QuantityInStock, m.SellingPrice })
                .SumAsync(x => (decimal?)(x.QuantityInStock * x.SellingPrice)) ?? 0m;

            return new InventoryStatusDto
            {
                TotalSkus = totalSkus,
                TotalBatches = totalBatches,
                NearExpiryBatches = nearExpiryBatches,
                ExpiredBatches = expiredBatches,
                TotalStockValue = stockValue,
            };
        }

        private async Task<List<LowStockMedicineDto>> BuildLowStockListAsync(int branchId)
        {
            return await _db.Inventories.AsNoTracking()
                .Where(i => i.BranchID == branchId && i.QuantityInStock <= i.ReorderLevel)
                .Join(_db.Medicines.AsNoTracking(),
                    i => i.MedicineID,
                    m => m.MedicineID,
                    (i, m) => new LowStockMedicineDto
                    {
                        MedicineId = m.MedicineID,
                        MedicineName = m.MedicineName,
                        Unit = m.Unit,
                        CurrentStock = i.QuantityInStock,
                        ReorderLevel = i.ReorderLevel,
                    })
                .OrderBy(x => x.CurrentStock - x.ReorderLevel)
                .ThenBy(x => x.MedicineName)
                .Take(20)
                .ToListAsync();
        }
    }
}
