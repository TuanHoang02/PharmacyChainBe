using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResponse<IEnumerable<UserResponse>>> GetAllUsersAsync(UserFilter filter)
        {
            var (users, totalRecords) = await _userRepository.GetAllAsync(filter);

            var mappedUsers = users.Select(u => new UserResponse
            {
                UserID = u.UserID,
                FullName = u.FullName,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleName = u.Role?.RoleName ?? string.Empty,
                BranchName = u.Branch?.BranchName,
                IsActive = u.IsActive
            });

            return new PagedResponse<IEnumerable<UserResponse>>
            {
                Data = mappedUsers,
                PageNumber = filter.Page,
                PageSize = filter.Size,
                TotalRecords = totalRecords
            };
        }

        public async Task<UserResponse> CreateUserAsync(UserRequest request)
        {
            await ValidateRoleNotSupplierAsync(request.RoleId);
            await ValidateDuplicateAsync(request.Username, request.Email, request.PhoneNumber);

            if (string.IsNullOrEmpty(request.Password))
            {
                throw new ApiException("Mật khẩu là bắt buộc đối với người dùng mới.", 400);
            }

            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleID = request.RoleId,
                BranchID = request.BranchID,
                IsActive = request.IsActive,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Re-fetch to get RoleName and BranchName if needed, but since we just return basic details, we can mock it
            // Or typically we do a GetById after insert. For simplicity, we'll fetch again.
            var fetchedUser = await _userRepository.GetByIdAsync(createdUser.UserID);

            return MapToResponse(fetchedUser ?? createdUser);
        }

        public async Task<UserResponse> UpdateUserAsync(int id, UserRequest request)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new ApiException("Không tìm thấy người dùng.", 404);
            }

            await ValidateRoleNotSupplierAsync(request.RoleId);
            await ValidateDuplicateAsync(request.Username, request.Email, request.PhoneNumber, id);

            existingUser.FullName = request.FullName;
            existingUser.Username = request.Username;
            existingUser.Email = request.Email;
            existingUser.PhoneNumber = request.PhoneNumber;
            existingUser.RoleID = request.RoleId;
            existingUser.BranchID = request.BranchID;
            existingUser.IsActive = request.IsActive;

            if (!string.IsNullOrEmpty(request.Password))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            
            var fetchedUser = await _userRepository.GetByIdAsync(updatedUser.UserID);
            return MapToResponse(fetchedUser ?? updatedUser);
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new ApiException("Không tìm thấy người dùng.", 404);
            }

            existingUser.IsActive = false;
            await _userRepository.UpdateAsync(existingUser);

            return true;
        }

        private async Task ValidateRoleNotSupplierAsync(int roleId)
        {
            var roles = await _userRepository.GetRolesAsync();
            var role = roles.FirstOrDefault(r => r.RoleID == roleId);
            if (role != null && role.RoleName == "Supplier")
            {
                throw new ApiException("Không được phép tạo hoặc cập nhật tài khoản cho nhà cung cấp (Supplier) từ chức năng này.", 403);
            }
        }

        private async Task ValidateDuplicateAsync(string username, string? email, string? phone, int? currentUserId = null)
        {
            var userByUsername = await _userRepository.GetByUsernameAsync(username);
            if (userByUsername != null && (!currentUserId.HasValue || userByUsername.UserID != currentUserId.Value))
            {
                throw new ApiException("Tên đăng nhập này đã được đăng ký cho một tài khoản khác.", 409);
            }

            if (!string.IsNullOrEmpty(email))
            {
                var userByEmail = await _userRepository.GetByEmailAsync(email);
                if (userByEmail != null && (!currentUserId.HasValue || userByEmail.UserID != currentUserId.Value))
                {
                    throw new ApiException("Email này đã được đăng ký cho một tài khoản khác.", 409);
                }
            }

            if (!string.IsNullOrEmpty(phone))
            {
                var userByPhone = await _userRepository.GetByPhoneAsync(phone);
                if (userByPhone != null && (!currentUserId.HasValue || userByPhone.UserID != currentUserId.Value))
                {
                    throw new ApiException("Số điện thoại này đã được đăng ký cho một tài khoản khác.", 409);
                }
            }
        }

        private UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.Role?.RoleName ?? string.Empty,
                BranchName = user.Branch?.BranchName,
                IsActive = user.IsActive
            };
        }

        public async Task<IEnumerable<LookupDto>> GetRolesAsync()
        {
            var roles = await _userRepository.GetRolesAsync();
            return roles
                .Where(r => r.RoleName != "Supplier")
                .Select(r => new LookupDto { Id = r.RoleID, Name = r.RoleName });
        }

        public async Task<IEnumerable<LookupDto>> GetBranchesAsync()
        {
            var branches = await _userRepository.GetBranchesAsync();
            return branches.Select(b => new LookupDto { Id = b.BranchID, Name = b.BranchName });
        }
    }
}
