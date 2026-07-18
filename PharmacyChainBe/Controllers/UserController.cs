using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Controllers
{
    [Route("api/administrator/users")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilter filter)
        {
            var pagedResponse = await _userService.GetAllUsersAsync(filter);
            
            return Ok(new BaseApiResponse<PagedResponse<IEnumerable<UserResponse>>>
            {
                Success = true,
                Message = "Lấy danh sách người dùng thành công.",
                Data = pagedResponse
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
        {
            var createdUser = await _userService.CreateUserAsync(request);
            
            return Ok(new BaseApiResponse<UserResponse>
            {
                Success = true,
                Message = "Tạo người dùng thành công.",
                Data = createdUser
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequest request)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, request);
            
            return Ok(new BaseApiResponse<UserResponse>
            {
                Success = true,
                Message = "Cập nhật người dùng thành công.",
                Data = updatedUser
            });
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var success = await _userService.DeactivateUserAsync(id);
            
            if (success)
            {
                return Ok(new BaseApiResponse<object>
                {
                    Success = true,
                    Message = "Vô hiệu hoá người dùng thành công.",
                    Data = null!
                });
            }

            return BadRequest(new BaseApiResponse<object>
            {
                Success = false,
                Message = "Vô hiệu hoá người dùng thất bại.",
                Data = null!
            });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userService.GetRolesAsync();
            return Ok(new BaseApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Lấy danh sách vai trò thành công.",
                Data = roles
            });
        }

        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _userService.GetBranchesAsync();
            return Ok(new BaseApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Lấy danh sách chi nhánh thành công.",
                Data = branches
            });
        }
    }
}
