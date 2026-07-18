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
                Message = "Users retrieved successfully.",
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
                Message = "User created successfully.",
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
                Message = "User updated successfully.",
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
                    Message = "User deactivated successfully.",
                    Data = null!
                });
            }

            return BadRequest(new BaseApiResponse<object>
            {
                Success = false,
                Message = "Failed to deactivate user.",
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
                Message = "Roles retrieved successfully.",
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
                Message = "Branches retrieved successfully.",
                Data = branches
            });
        }
    }
}
