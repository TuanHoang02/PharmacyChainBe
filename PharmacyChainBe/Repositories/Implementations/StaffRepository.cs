using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetStaffsAsync(
            string? searchTerm, 
            int? branchId, 
            int? roleId, 
            bool? isActive, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .AsQueryable();

            // Lọc nhân viên chi nhánh: loại bỏ các role như Admin/OperationsManager nếu cần, 
            // nhưng ở đây cứ lấy theo điều kiện lọc branchId.
            // Nếu là quản lý chi nhánh, branchId được truyền vào từ Service để bắt buộc lọc.
            if (branchId.HasValue)
            {
                query = query.Where(u => u.BranchID == branchId.Value);
            }
            else
            {
                // Nếu Admin/OpsManager gọi mà không truyền branchId, thì lấy những ai có BranchID hoặc lọc chung
                // (Chỉ quản lý nhân viên chi nhánh, có thể bao gồm cả người không có chi nhánh, tuy nhiên chủ yếu là có chi nhánh)
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleID == roleId.Value);
            }
            else
            {
                // Chỉ lấy Pharmacist (3) và BranchManager (4) dưới quyền quản lý
                query = query.Where(u => u.RoleID == 3 || u.RoleID == 4);
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(search) 
                                      || u.Username.ToLower().Contains(search)
                                      || (u.Email != null && u.Email.ToLower().Contains(search))
                                      || (u.PhoneNumber != null && u.PhoneNumber.Contains(search)));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<User?> GetStaffByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<User> AddStaffAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateStaffAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null)
        {
            var query = _context.Users.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.UserID != excludeId.Value);
            }
            return await query.AnyAsync(u => u.Username.ToLower() == username.Trim().ToLower());
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            var query = _context.Users.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.UserID != excludeId.Value);
            }
            return await query.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.Trim().ToLower());
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == roleId);
        }

        public async Task<bool> BranchExistsAsync(int branchId)
        {
            return await _context.Branches.AnyAsync(b => b.BranchID == branchId);
        }
    }
}
