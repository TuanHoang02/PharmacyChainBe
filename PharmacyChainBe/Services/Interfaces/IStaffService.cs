using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IStaffService
    {
        Task<PagedResponse<IEnumerable<StaffDto>>> GetStaffsAsync(
            string? searchTerm, 
            int? branchId, 
            int? roleId, 
            bool? isActive, 
            int pageNumber, 
            int pageSize, 
            int? currentUserBranchId, 
            string currentUserRole);

        Task<StaffDto> GetStaffByIdAsync(int id, int? currentUserBranchId, string currentUserRole);
        Task<StaffDto> CreateStaffAsync(CreateStaffDto request, int? currentUserBranchId, string currentUserRole);
        Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffDto request, int? currentUserBranchId, string currentUserRole);
        Task<bool> DeleteStaffAsync(int id, int? currentUserBranchId, string currentUserRole);
    }
}
