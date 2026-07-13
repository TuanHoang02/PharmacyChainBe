using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(new BaseApiResponse<AuthResponseDto> { Success = true, Message = "Login successful.", Data = response });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(new BaseApiResponse<AuthResponseDto> { Success = true, Message = "Registration successful.", Data = response });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new BaseApiResponse<object> { Success = true, Message = "Logout successful." });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized(new BaseApiResponse<object> { Success = false, Message = "Không thể xác thực người dùng." });
            }

            var success = await _authService.ChangePasswordAsync(userId, request);
            if (success)
            {
                return Ok(new BaseApiResponse<object> { Success = true, Message = "Đổi mật khẩu thành công." });
            }
            
            return BadRequest(new BaseApiResponse<object> { Success = false, Message = "Đổi mật khẩu thất bại." });
        }
    }
}
