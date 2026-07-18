using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;
using System.Security.Claims;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "OperationsManager,BranchManager")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffs(
            [FromQuery] string? searchTerm,
            [FromQuery] int? branchId,
            [FromQuery] int? roleId,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var (currentUserRole, currentUserBranchId, _) = GetCurrentUserClaims();

            var result = await _staffService.GetStaffsAsync(
                searchTerm, 
                branchId, 
                roleId, 
                isActive, 
                pageNumber, 
                pageSize, 
                currentUserBranchId, 
                currentUserRole);

            return Ok(new BaseApiResponse<PagedResponse<IEnumerable<StaffDto>>>
            {
                Success = true,
                Message = "Lấy danh sách nhân viên thành công.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var (currentUserRole, currentUserBranchId, _) = GetCurrentUserClaims();
            var result = await _staffService.GetStaffByIdAsync(id, currentUserBranchId, currentUserRole);
            return Ok(new BaseApiResponse<StaffDto>
            {
                Success = true,
                Message = "Lấy chi tiết nhân viên thành công.",
                Data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto request)
        {
            var (currentUserRole, currentUserBranchId, _) = GetCurrentUserClaims();
            var result = await _staffService.CreateStaffAsync(request, currentUserBranchId, currentUserRole);
            return CreatedAtAction(nameof(GetStaffById), new { id = result.UserID }, new BaseApiResponse<StaffDto>
            {
                Success = true,
                Message = "Tạo tài khoản nhân viên thành công.",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffDto request)
        {
            var (currentUserRole, currentUserBranchId, _) = GetCurrentUserClaims();
            var result = await _staffService.UpdateStaffAsync(id, request, currentUserBranchId, currentUserRole);
            return Ok(new BaseApiResponse<StaffDto>
            {
                Success = true,
                Message = "Cập nhật tài khoản nhân viên thành công.",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var (currentUserRole, currentUserBranchId, currentUserId) = GetCurrentUserClaims();

            // Ngăn chặn tự xóa tài khoản của chính mình đang đăng nhập
            if (id == currentUserId)
            {
                return BadRequest(new BaseApiResponse<object>
                {
                    Success = false,
                    Message = "Bạn không thể tự xóa tài khoản của chính mình."
                });
            }

            var result = await _staffService.DeleteStaffAsync(id, currentUserBranchId, currentUserRole);
            if (result)
            {
                return Ok(new BaseApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa nhân viên thành công (Xóa mềm)."
                });
            }

            return BadRequest(new BaseApiResponse<object>
            {
                Success = false,
                Message = "Xóa nhân viên thất bại."
            });
        }

        private (string Role, int? BranchId, int UserId) GetCurrentUserClaims()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            
            int? branchId = null;
            var branchIdStr = User.FindFirst("BranchID")?.Value;
            if (!string.IsNullOrEmpty(branchIdStr) && int.TryParse(branchIdStr, out int bId))
            {
                branchId = bId;
            }

            int userId = 0;
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int uId))
            {
                userId = uId;
            }

            return (role, branchId, userId);
        }
    }
}
