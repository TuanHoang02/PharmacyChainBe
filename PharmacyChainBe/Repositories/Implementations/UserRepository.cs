using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<User> Users, int TotalRecords)> GetAllAsync(UserFilter filter)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                var lowerKeyword = filter.Keyword.ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(lowerKeyword) || 
                                         u.Username.ToLower().Contains(lowerKeyword) ||
                                         (u.Email != null && u.Email.ToLower().Contains(lowerKeyword)) ||
                                         (u.PhoneNumber != null && u.PhoneNumber.Contains(lowerKeyword)));
            }

            if (filter.RoleId.HasValue)
            {
                query = query.Where(u => u.RoleID == filter.RoleId.Value);
            }

            if (filter.BranchID.HasValue)
            {
                query = query.Where(u => u.BranchID == filter.BranchID.Value);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            var totalRecords = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return (users, totalRecords);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            return await _context.Branches.Where(b => b.IsActive).ToListAsync();
        }
    }
}
