using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IBranchPerformanceService
    {
        Task<BranchPerformanceResponse> GetPerformanceDataAsync(int? branchId, string period, DateTime? startDate = null, DateTime? endDate = null);
    }
}
