using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class BranchPerformanceRepository : IBranchPerformanceRepository
    {
        private readonly AppDbContext _context;

        public BranchPerformanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate, int? branchId)
        {
            var query = _context.SalesInvoices
                .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate);

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(i => i.BranchID == branchId.Value);
            }

            return await query.SumAsync(i => i.TotalAmount);
        }

        public async Task<int> GetExpiredMedicineCountAsync(DateTime endDate, int? branchId)
        {
            var query = _context.MedicineBatches
                .Where(m => m.ExpiryDate < endDate && m.RemainingQuantity > 0);

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(m => m.BranchID == branchId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<(DateTime Date, decimal Total)>> GetSalesGroupedByDateAsync(DateTime startDate, DateTime endDate, int? branchId)
        {
            var query = _context.SalesInvoices
                .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate);

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(i => i.BranchID == branchId.Value);
            }

            var grouped = await query
                .GroupBy(i => i.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            return grouped.Select(g => (g.Date, g.Total)).ToList();
        }

        public async Task<List<Branch>> GetActiveBranchesAsync()
        {
            return await _context.Branches.Where(b => b.IsActive).ToListAsync();
        }

        public async Task<Dictionary<int, decimal>> GetTotalSalesByBranchAsync(DateTime startDate, DateTime endDate)
        {
            var grouped = await _context.SalesInvoices
                .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
                .GroupBy(i => i.BranchID)
                .Select(g => new { BranchId = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            return grouped.ToDictionary(g => g.BranchId ?? 0, g => g.Total);
        }

        public async Task<Dictionary<int, int>> GetExpiredMedicineCountByBranchAsync(DateTime endDate)
        {
            var grouped = await _context.MedicineBatches
                .Where(m => m.ExpiryDate < endDate && m.RemainingQuantity > 0)
                .GroupBy(m => m.BranchID)
                .Select(g => new { BranchId = g.Key, Count = g.Count() })
                .ToListAsync();

            return grouped.ToDictionary(g => g.BranchId ?? 0, g => g.Count);
        }
    }
}
