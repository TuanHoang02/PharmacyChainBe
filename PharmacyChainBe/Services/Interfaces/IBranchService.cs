using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IBranchService
    {
        Task<PagedResponse<IEnumerable<BranchDto>>> GetBranchesAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
        Task<BranchDto> GetBranchByIdAsync(int id);
        Task<BranchDto> CreateBranchAsync(CreateBranchDto request);
        Task<BranchDto> UpdateBranchAsync(int id, UpdateBranchDto request);
        Task<bool> DeleteBranchAsync(int id);
    }
}
