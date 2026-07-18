using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<IEnumerable<UserResponse>>> GetAllUsersAsync(UserFilter filter);
        Task<UserResponse> CreateUserAsync(UserRequest request);
        Task<UserResponse> UpdateUserAsync(int id, UserRequest request);
        Task<bool> DeactivateUserAsync(int id);
        Task<IEnumerable<LookupDto>> GetRolesAsync();
        Task<IEnumerable<LookupDto>> GetBranchesAsync();
    }
}
