using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly AppDbContext _context;

        public SupplierService(ISupplierRepository supplierRepository, AppDbContext context)
        {
            _supplierRepository = supplierRepository;
            _context = context;
        }

        public async Task<PagedResponse<IEnumerable<SupplierDto>>> GetSuppliersAsync(
            string? searchTerm, 
            bool? isActive, 
            int pageNumber, 
            int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (items, totalCount) = await _supplierRepository.GetSuppliersAsync(searchTerm, isActive, pageNumber, pageSize);

            var dtos = items.Select(s => MapToDto(s));

            return new PagedResponse<IEnumerable<SupplierDto>>
            {
                Data = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount
            };
        }

        public async Task<SupplierDto> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                throw new ApiException("Nhà cung cấp không tồn tại.", 404);
            }

            return MapToDto(supplier);
        }

        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto request)
        {
            var isDuplicate = await _supplierRepository.ExistsByNameAsync(request.SupplierName);
            if (isDuplicate)
            {
                throw new ApiException("Tên nhà cung cấp đã tồn tại.", 400);
            }

            var supplier = new Supplier
            {
                SupplierName = request.SupplierName.Trim(),
                ContactName = request.ContactName?.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                Email = request.Email?.Trim(),
                Address = request.Address?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdSupplier = await _supplierRepository.AddSupplierAsync(supplier);
            return MapToDto(createdSupplier);
        }

        public async Task<SupplierDto> CreateSupplierWithUserAsync(CreateSupplierWithUserDto request)
        {
            // 1. Kiểm tra trùng lặp tên nhà cung cấp
            var isDuplicateSupplierName = await _supplierRepository.ExistsByNameAsync(request.SupplierName);
            if (isDuplicateSupplierName)
            {
                throw new ApiException("Tên nhà cung cấp đã tồn tại.", 400);
            }

            // 2. Kiểm tra trùng lặp tên đăng nhập (Username)
            var isDuplicateUsername = await _context.Users.AnyAsync(u => u.Username.ToLower() == request.Username.Trim().ToLower());
            if (isDuplicateUsername)
            {
                throw new ApiException("Tên đăng nhập đã tồn tại.", 400);
            }

            // 3. Kiểm tra trùng lặp Email (nếu có nhập)
            if (!string.IsNullOrEmpty(request.Email))
            {
                var isDuplicateEmail = await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == request.Email.Trim().ToLower());
                if (isDuplicateEmail)
                {
                    throw new ApiException("Email đã tồn tại.", 400);
                }
            }

            // 4. Lấy RoleID của Supplier
            var supplierRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Supplier");
            if (supplierRole == null)
            {
                throw new ApiException("Không tìm thấy vai trò 'Supplier' trong hệ thống.", 500);
            }

            // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. Tạo mới Supplier
                var supplier = new Supplier
                {
                    SupplierName = request.SupplierName.Trim(),
                    ContactName = request.ContactName?.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim(),
                    Email = request.Email?.Trim(),
                    Address = request.Address?.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                // B. Tạo mới User
                var user = new User
                {
                    Username = request.Username.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FullName = request.SupplierName.Trim(),
                    Email = request.Email?.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim(),
                    RoleID = supplierRole.RoleID,
                    SupplierID = supplier.SupplierID,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Load lại Supplier kèm theo Users để map sang DTO
                var createdSupplier = await _context.Suppliers
                    .Include(s => s.Users)
                    .FirstOrDefaultAsync(s => s.SupplierID == supplier.SupplierID);

                return MapToDto(createdSupplier!);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SupplierDto> UpdateSupplierAsync(int id, UpdateSupplierDto request)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                throw new ApiException("Nhà cung cấp không tồn tại.", 404);
            }

            var isDuplicate = await _supplierRepository.ExistsByNameAsync(request.SupplierName, id);
            if (isDuplicate)
            {
                throw new ApiException("Tên nhà cung cấp đã tồn tại.", 400);
            }

            supplier.SupplierName = request.SupplierName.Trim();
            supplier.ContactName = request.ContactName?.Trim();
            supplier.PhoneNumber = request.PhoneNumber?.Trim();
            supplier.Email = request.Email?.Trim();
            supplier.Address = request.Address?.Trim();
            supplier.IsActive = request.IsActive;

            await _supplierRepository.UpdateSupplierAsync(supplier);
            return MapToDto(supplier);
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                throw new ApiException("Nhà cung cấp không tồn tại.", 404);
            }

            // Soft delete
            supplier.IsActive = false;
            return await _supplierRepository.UpdateSupplierAsync(supplier);
        }

        private static SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                ContactName = supplier.ContactName,
                PhoneNumber = supplier.PhoneNumber,
                Email = supplier.Email,
                Address = supplier.Address,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt,
                Username = supplier.Users?.FirstOrDefault()?.Username
            };
        }
    }
}
