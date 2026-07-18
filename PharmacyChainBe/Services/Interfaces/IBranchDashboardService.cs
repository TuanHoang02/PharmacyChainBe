using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IBranchDashboardService
    {
        Task<BranchDashboardDto> GetDashboardAsync(int branchId);
    }
}
