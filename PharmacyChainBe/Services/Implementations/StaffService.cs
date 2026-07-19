using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<PagedResponse<IEnumerable<StaffDto>>> GetStaffsAsync(
            string? searchTerm, 
            int? branchId, 
            int? roleId, 
            bool? isActive, 
            int pageNumber, 
            int pageSize, 
            int? currentUserBranchId, 
            string currentUserRole)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            // Kiểm tra phân quyền: BranchManager chỉ được quản lý chi nhánh của mình
            if (currentUserRole != "OperationsManager" && currentUserRole != "Operations Manager")
            {
                if (currentUserRole == "BranchManager" || currentUserRole == "Branch Manager")
                {
                    branchId = currentUserBranchId; // Bắt buộc lọc theo chi nhánh của Manager
                    roleId = 4; // BranchManager chỉ được quản lý Pharmacist (Dược sĩ) của chi nhánh mình
                }
                else
                {
                    throw new ApiException("Bạn không có quyền xem thông tin nhân viên.", 403);
                }
            }

            var (items, totalCount) = await _staffRepository.GetStaffsAsync(searchTerm, branchId, roleId, isActive, pageNumber, pageSize);

            var dtos = items.Select(u => MapToDto(u));

            return new PagedResponse<IEnumerable<StaffDto>>
            {
                Data = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount
            };
        }

        public async Task<StaffDto> GetStaffByIdAsync(int id, int? currentUserBranchId, string currentUserRole)
        {
            var user = await _staffRepository.GetStaffByIdAsync(id);
            if (user == null)
            {
                throw new ApiException("Nhân viên không tồn tại.", 404);
            }

            // Kiểm tra phân quyền
            if (currentUserRole != "OperationsManager" && currentUserRole != "Operations Manager")
            {
                if (currentUserRole == "BranchManager" || currentUserRole == "Branch Manager")
                {
                    if (user.BranchID != currentUserBranchId)
                    {
                        throw new ApiException("Bạn không có quyền truy cập nhân viên ở chi nhánh khác.", 403);
                    }
                }
                else
                {
                    throw new ApiException("Bạn không có quyền xem thông tin nhân viên.", 403);
                }
            }

            return MapToDto(user);
        }

        public async Task<StaffDto> CreateStaffAsync(CreateStaffDto request, int? currentUserBranchId, string currentUserRole)
        {
            // Kiểm tra phân quyền khi gán chi nhánh
            int? targetBranchId = request.BranchID;
            
            if (currentUserRole != "OperationsManager" && currentUserRole != "Operations Manager")
            {
                if (currentUserRole == "BranchManager" || currentUserRole == "Branch Manager")
                {
                    targetBranchId = currentUserBranchId; // BranchManager chỉ được tạo nhân viên cho chính chi nhánh của mình
                }
                else
                {
                    throw new ApiException("Bạn không có quyền tạo nhân viên.", 403);
                }
            }

            // Kiểm tra xem RoleID có tồn tại không
            var role = await _staffRepository.GetRoleByIdAsync(request.RoleID);
            if (role == null)
            {
                throw new ApiException("Quyền (RoleID) không hợp lệ.", 400);
            }

            // Ngăn chặn BranchManager tạo tài khoản Admin hoặc OperationsManager
            if ((currentUserRole == "BranchManager" || currentUserRole == "Branch Manager") && (role.RoleName == "Admin" || role.RoleName == "Administrator" || role.RoleName == "OperationsManager" || role.RoleName == "Operations Manager"))
            {
                throw new ApiException("Bạn không được phép gán quyền quản trị cấp cao cho tài khoản mới.", 403);
            }

            // Kiểm tra xem chi nhánh có tồn tại không
            if (targetBranchId.HasValue)
            {
                var branchExists = await _staffRepository.BranchExistsAsync(targetBranchId.Value);
                if (!branchExists)
                {
                    throw new ApiException("Chi nhánh không tồn tại.", 400);
                }
            }

            // Kiểm tra trùng lặp tài khoản
            var isDuplicateUsername = await _staffRepository.ExistsByUsernameAsync(request.Username);
            if (isDuplicateUsername)
            {
                throw new ApiException("Tên đăng nhập đã tồn tại.", 400);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                var isDuplicateEmail = await _staffRepository.ExistsByEmailAsync(request.Email);
                if (isDuplicateEmail)
                {
                    throw new ApiException("Email đã tồn tại.", 400);
                }
            }

            var newUser = new User
            {
                Username = request.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName.Trim(),
                Email = request.Email?.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                RoleID = request.RoleID,
                BranchID = targetBranchId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _staffRepository.AddStaffAsync(newUser);
            
            // Lấy lại thông tin đầy đủ để map kèm RoleName & BranchName
            var dbUser = await _staffRepository.GetStaffByIdAsync(createdUser.UserID);
            return MapToDto(dbUser!);
        }

        public async Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffDto request, int? currentUserBranchId, string currentUserRole)
        {
            var user = await _staffRepository.GetStaffByIdAsync(id);
            if (user == null)
            {
                throw new ApiException("Nhân viên không tồn tại.", 404);
            }

            // Kiểm tra phân quyền đối với bản ghi cần chỉnh sửa
            if (currentUserRole != "OperationsManager" && currentUserRole != "Operations Manager")
            {
                if (currentUserRole == "BranchManager" || currentUserRole == "Branch Manager")
                {
                    if (user.BranchID != currentUserBranchId)
                    {
                        throw new ApiException("Bạn không có quyền cập nhật nhân viên chi nhánh khác.", 403);
                    }
                }
                else
                {
                    throw new ApiException("Bạn không có quyền cập nhật nhân viên.", 403);
                }
            }

            // Kiểm tra RoleID hợp lệ
            var role = await _staffRepository.GetRoleByIdAsync(request.RoleID);
            if (role == null)
            {
                throw new ApiException("Quyền (RoleID) không hợp lệ.", 400);
            }

            // Ngăn chặn BranchManager thăng cấp thành Admin/OpsManager hoặc hạ cấp bản thân nếu tự sửa
            if (currentUserRole == "BranchManager" || currentUserRole == "Branch Manager")
            {
                if (role.RoleName == "Admin" || role.RoleName == "Administrator" || role.RoleName == "OperationsManager" || role.RoleName == "Operations Manager")
                {
                    throw new ApiException("Bạn không được phép gán quyền quản trị cấp cao.", 403);
                }
            }

            // Xác định BranchID đích
            int? targetBranchId = request.BranchID;
            if (currentUserRole == "BranchManager" || currentUserRole == "Branch Manager")
            {
                targetBranchId = currentUserBranchId; // Bắt buộc giữ nguyên chi nhánh của quản lý đó
            }

            // Kiểm tra chi nhánh tồn tại
            if (targetBranchId.HasValue)
            {
                var branchExists = await _staffRepository.BranchExistsAsync(targetBranchId.Value);
                if (!branchExists)
                {
                    throw new ApiException("Chi nhánh không tồn tại.", 400);
                }
            }

            // Kiểm tra trùng Email
            if (!string.IsNullOrEmpty(request.Email))
            {
                var isDuplicateEmail = await _staffRepository.ExistsByEmailAsync(request.Email, id);
                if (isDuplicateEmail)
                {
                    throw new ApiException("Email đã tồn tại.", 400);
                }
            }

            user.FullName = request.FullName.Trim();
            user.Email = request.Email?.Trim();
            user.PhoneNumber = request.PhoneNumber?.Trim();
            user.RoleID = request.RoleID;
            user.BranchID = targetBranchId;
            user.IsActive = request.IsActive;

            await _staffRepository.UpdateStaffAsync(user);

            var dbUser = await _staffRepository.GetStaffByIdAsync(user.UserID);
            return MapToDto(dbUser!);
        }

        public async Task<bool> DeleteStaffAsync(int id, int? currentUserBranchId, string currentUserRole)
        {
            var user = await _staffRepository.GetStaffByIdAsync(id);
            if (user == null)
            {
                throw new ApiException("Nhân viên không tồn tại.", 404);
            }

            // Kiểm tra phân quyền xóa
            if (currentUserRole != "OperationsManager" && currentUserRole != "Operations Manager")
            {
                if (currentUserRole == "BranchManager" || currentUserRole == "Branch Manager")
                {
                    if (user.BranchID != currentUserBranchId)
                    {
                        throw new ApiException("Bạn không có quyền xóa nhân viên chi nhánh khác.", 403);
                    }
                }
                else
                {
                    throw new ApiException("Bạn không có quyền xóa nhân viên.", 403);
                }
            }

            // Ngăn tự xóa tài khoản của chính mình đang đăng nhập
            // (Chỗ này có thể kiểm tra cụ thể ID ở Controller, ở đây thực hiện xóa mềm)
            user.IsActive = false;
            return await _staffRepository.UpdateStaffAsync(user);
        }

        private static StaffDto MapToDto(User user)
        {
            return new StaffDto
            {
                UserID = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleID = user.RoleID,
                RoleName = user.Role?.RoleName ?? string.Empty,
                BranchID = user.BranchID,
                BranchName = user.Branch?.BranchName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
