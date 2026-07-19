using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalRevenueAsync(int? branchId, DateTime startDate, DateTime endDate)
        {
            var query = _context.SalesInvoices
                .Where(si => si.InvoiceStatus == InvoiceStatus.Finalized 
                          && si.CreatedAt >= startDate 
                          && si.CreatedAt <= endDate);

            if (branchId.HasValue)
            {
                query = query.Where(si => si.BranchID == branchId.Value);
            }

            return await query.SumAsync(si => si.TotalAmount);
        }

        public async Task<int> GetSalesCountAsync(int? branchId, DateTime startDate, DateTime endDate)
        {
            var query = _context.SalesInvoices
                .Where(si => si.InvoiceStatus == InvoiceStatus.Finalized 
                          && si.CreatedAt >= startDate 
                          && si.CreatedAt <= endDate);

            if (branchId.HasValue)
            {
                query = query.Where(si => si.BranchID == branchId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<decimal> GetTotalPurchaseExpenseAsync(int? branchId, DateTime startDate, DateTime endDate)
        {
            var query = _context.PurchaseOrders
                .Where(po => po.OrderStatus == OrderStatus.Completed 
                          && po.CreatedAt >= startDate 
                          && po.CreatedAt <= endDate);

            if (branchId.HasValue)
            {
                query = query.Where(po => po.BranchID == branchId.Value);
            }

            return await query.SumAsync(po => po.TotalAmount);
        }

        public async Task<List<DailyRevenueDto>> GetDailyStatisticsAsync(int? branchId, DateTime startDate, DateTime endDate)
        {
            // Lấy hóa đơn bán hàng theo ngày
            var salesQuery = _context.SalesInvoices
                .Where(si => si.InvoiceStatus == InvoiceStatus.Finalized 
                          && si.CreatedAt >= startDate 
                          && si.CreatedAt <= endDate);

            if (branchId.HasValue)
            {
                salesQuery = salesQuery.Where(si => si.BranchID == branchId.Value);
            }

            var dailySales = await salesQuery
                .GroupBy(si => si.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(si => si.TotalAmount) })
                .ToListAsync();

            // Lấy đơn nhập hàng theo ngày
            var purchaseQuery = _context.PurchaseOrders
                .Where(po => po.OrderStatus == OrderStatus.Completed 
                          && po.CreatedAt >= startDate 
                          && po.CreatedAt <= endDate);

            if (branchId.HasValue)
            {
                purchaseQuery = purchaseQuery.Where(po => po.BranchID == branchId.Value);
            }

            var dailyPurchases = await purchaseQuery
                .GroupBy(po => po.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(po => po.TotalAmount) })
                .ToListAsync();

            // Tổ hợp kết quả theo từng ngày
            var allDates = dailySales.Select(s => s.Date)
                .Union(dailyPurchases.Select(p => p.Date))
                .OrderBy(d => d)
                .ToList();

            var result = new List<DailyRevenueDto>();
            foreach (var date in allDates)
            {
                result.Add(new DailyRevenueDto
                {
                    Date = date,
                    Revenue = dailySales.FirstOrDefault(s => s.Date == date)?.Amount ?? 0,
                    Expense = dailyPurchases.FirstOrDefault(p => p.Date == date)?.Amount ?? 0
                });
            }

            return result;
        }

        public async Task<List<LowStockDto>> GetLowStockMedicinesAsync(int? branchId)
        {
            var query = _context.Inventories
                .Include(i => i.Medicine)
                .Include(i => i.Branch)
                .Where(i => i.QuantityInStock <= i.ReorderLevel);

            if (branchId.HasValue)
            {
                query = query.Where(i => i.BranchID == branchId.Value);
            }

            return await query
                .Select(i => new LowStockDto
                {
                    MedicineID = i.MedicineID,
                    MedicineName = i.Medicine != null ? i.Medicine.MedicineName : string.Empty,
                    QuantityInStock = i.QuantityInStock,
                    ReorderLevel = i.ReorderLevel,
                    BranchName = i.Branch != null ? i.Branch.BranchName : string.Empty
                })
                .ToListAsync();
        }

        public async Task<List<ExpiringBatchDto>> GetExpiringBatchesAsync(int? branchId, int daysThreshold)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysThreshold);

            var query = _context.MedicineBatches
                .Include(mb => mb.Medicine)
                .Include(mb => mb.Branch)
                .Where(mb => mb.RemainingQuantity > 0 
                          && mb.ExpiryDate <= targetDate);

            if (branchId.HasValue)
            {
                query = query.Where(mb => mb.BranchID == branchId.Value);
            }

            var items = await query.ToListAsync();

            return items.Select(mb => new ExpiringBatchDto
            {
                MedicineBatchID = mb.MedicineBatchID,
                BatchNumber = mb.BatchNumber,
                MedicineName = mb.Medicine != null ? mb.Medicine.MedicineName : string.Empty,
                BranchName = mb.Branch != null ? mb.Branch.BranchName : string.Empty,
                ExpiryDate = mb.ExpiryDate,
                RemainingQuantity = mb.RemainingQuantity,
                DaysUntilExpiry = (mb.ExpiryDate - DateTime.UtcNow.Date).Days
            }).ToList();
        }

        public async Task<List<TopSellingDto>> GetTopSellingMedicinesAsync(
            int? branchId, 
            DateTime startDate, 
            DateTime endDate, 
            int limit)
        {
            var query = _context.SalesInvoiceDetails
                .Include(sid => sid.SalesInvoice)
                .Include(sid => sid.Medicine)
                .ThenInclude(m => m!.Category)
                .Where(sid => sid.SalesInvoice!.InvoiceStatus == InvoiceStatus.Finalized 
                           && sid.SalesInvoice.CreatedAt >= startDate 
                           && sid.SalesInvoice.CreatedAt <= endDate);

            if (branchId.HasValue)
            {
                query = query.Where(sid => sid.SalesInvoice!.BranchID == branchId.Value);
            }

            var groupResult = await query
                .GroupBy(sid => new { sid.MedicineID, sid.Medicine!.MedicineName, CategoryName = sid.Medicine.Category != null ? sid.Medicine.Category.CategoryName : null })
                .Select(g => new TopSellingDto
                {
                    MedicineID = g.Key.MedicineID,
                    MedicineName = g.Key.MedicineName,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(sid => sid.Quantity),
                    TotalRevenueGenerated = g.Sum(sid => sid.LineTotal)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(limit)
                .ToListAsync();

            return groupResult;
        }

        public async Task<List<BranchPerformanceDto>> GetBranchPerformanceAsync(DateTime startDate, DateTime endDate)
        {
            var performance = await _context.Branches
                .Select(b => new BranchPerformanceDto
                {
                    BranchID = b.BranchID,
                    BranchName = b.BranchName,
                    Revenue = b.SalesInvoices
                        .Where(si => si.InvoiceStatus == InvoiceStatus.Finalized 
                                  && si.CreatedAt >= startDate 
                                  && si.CreatedAt <= endDate)
                        .Sum(si => si.TotalAmount),
                    SalesCount = b.SalesInvoices
                        .Count(si => si.InvoiceStatus == InvoiceStatus.Finalized 
                                  && si.CreatedAt >= startDate 
                                  && si.CreatedAt <= endDate)
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            return performance;
        }

        public async Task<decimal> GetCostOfGoodsSoldAsync(int? branchId, DateTime startDate, DateTime endDate)
        {
            var query = _context.SalesInvoiceDetails
                .Include(sid => sid.SalesInvoice)
                .Include(sid => sid.Medicine)
                .Where(sid => sid.SalesInvoice!.InvoiceStatus == InvoiceStatus.Finalized 
                           && sid.SalesInvoice.CreatedAt >= startDate 
                           && sid.SalesInvoice.CreatedAt <= endDate);

            if (branchId.HasValue)
            {
                query = query.Where(sid => sid.SalesInvoice!.BranchID == branchId.Value);
            }

            // COGS = Sum of (Quantity * Medicine.ImportPrice)
            // Đảm bảo ép kiểu decimal khi tính toán trong Db Query
            return await query.SumAsync(sid => (decimal)sid.Quantity * (sid.Medicine != null ? sid.Medicine.ImportPrice : 0m));
        }
    }
}
