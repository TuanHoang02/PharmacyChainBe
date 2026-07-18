using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IBranchRepository
    {
        Task<(IEnumerable<Branch> Items, int TotalCount)> GetBranchesAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
        Task<Branch?> GetBranchByIdAsync(int id);
        Task<Branch?> GetBranchByNameAsync(string name);
        Task<Branch> AddBranchAsync(Branch branch);
        Task<bool> UpdateBranchAsync(Branch branch);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
