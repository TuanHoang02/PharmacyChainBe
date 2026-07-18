using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IBranchPerformanceRepository
    {
        Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate, int? branchId);
        Task<List<(DateTime Date, decimal Total)>> GetSalesGroupedByDateAsync(DateTime startDate, DateTime endDate, int? branchId);
        Task<List<Branch>> GetActiveBranchesAsync();
        Task<Dictionary<int, decimal>> GetTotalSalesByBranchAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalInvoicesAsync(DateTime startDate, DateTime endDate, int? branchId);
        Task<int> GetLowStockMedicinesCountAsync(int? branchId);
        Task<Dictionary<int, int>> GetTotalInvoicesByBranchAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetLowStockMedicinesCountByBranchAsync();
    }
}
