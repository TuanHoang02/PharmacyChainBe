using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<decimal> GetTotalRevenueAsync(int? branchId, DateTime startDate, DateTime endDate);
        Task<int> GetSalesCountAsync(int? branchId, DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalPurchaseExpenseAsync(int? branchId, DateTime startDate, DateTime endDate);
        Task<List<DailyRevenueDto>> GetDailyStatisticsAsync(int? branchId, DateTime startDate, DateTime endDate);
        Task<List<LowStockDto>> GetLowStockMedicinesAsync(int? branchId);
        Task<List<ExpiringBatchDto>> GetExpiringBatchesAsync(int? branchId, int daysThreshold);
        Task<List<TopSellingDto>> GetTopSellingMedicinesAsync(int? branchId, DateTime startDate, DateTime endDate, int limit);
        Task<List<BranchPerformanceDto>> GetBranchPerformanceAsync(DateTime startDate, DateTime endDate);
    }
}
