using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<(IEnumerable<User> Users, int TotalRecords)> GetAllAsync(UserFilter filter);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneAsync(string phone);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<IEnumerable<Branch>> GetBranchesAsync();
    }
}
