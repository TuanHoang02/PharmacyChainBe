using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(
            int? branchId, 
            DateTime? startDate, 
            DateTime? endDate, 
            int? currentUserBranchId, 
            string currentUserRole);
    }
}
