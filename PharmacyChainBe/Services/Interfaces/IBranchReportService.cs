using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IBranchReportService
    {
        Task<BranchReportResponseDto> GetSalesReportAsync(int branchId, DateTime startDate, DateTime endDate);
        Task<BranchReportResponseDto> GetInventoryReportAsync(int branchId);
    }
}
