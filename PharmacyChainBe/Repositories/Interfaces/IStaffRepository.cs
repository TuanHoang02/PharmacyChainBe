using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<(IEnumerable<User> Items, int TotalCount)> GetStaffsAsync(string? searchTerm, int? branchId, int? roleId, bool? isActive, int pageNumber, int pageSize);
        Task<User?> GetStaffByIdAsync(int id);
        Task<User> AddStaffAsync(User user);
        Task<bool> UpdateStaffAsync(User user);
        Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task<bool> BranchExistsAsync(int branchId);
    }
}
