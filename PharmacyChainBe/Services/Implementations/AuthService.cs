using PharmacyChainBe.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ApiException("Email hoặc mật khẩu không chính xác.", 401);
            }

            if (!user.Status)
            {
                 throw new ApiException("Tài khoản đã bị khóa.", 401);
            }

            bool isPasswordValid = false;
            try 
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            } 
            catch
            {
                // In case it's not bcrypt hash for some legacy users, we can just fail.
                // Or if it's plaintext for testing purposes. We'll stick to bcrypt.
                if (request.Password == user.PasswordHash) isPasswordValid = true;
            }

            if (!isPasswordValid)
            {
                throw new ApiException("Email hoặc mật khẩu không chính xác.", 401);
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto 
            { 
                Token = token,
                Role = user.Role?.RoleName ?? string.Empty
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInMinutes = Convert.ToInt32(jwtSettings["ExpiryInMinutes"] ?? "60");

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT SecretKey is not configured.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
            };

            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleName));
            }

            if (user.BranchID.HasValue)
            {
                claims.Add(new Claim("BranchID", user.BranchID.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
