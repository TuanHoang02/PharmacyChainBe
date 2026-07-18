using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class BranchRepository : IBranchRepository
    {
        private readonly AppDbContext _context;

        public BranchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Branch> Items, int TotalCount)> GetBranchesAsync(
            string? searchTerm, 
            bool? isActive, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Branches.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(b => b.BranchName.ToLower().Contains(search) 
                                      || b.Address.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(b => b.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Branch?> GetBranchByIdAsync(int id)
        {
            return await _context.Branches.FirstOrDefaultAsync(b => b.BranchID == id);
        }

        public async Task<Branch?> GetBranchByNameAsync(string name)
        {
            return await _context.Branches
                .FirstOrDefaultAsync(b => b.BranchName.ToLower() == name.Trim().ToLower());
        }

        public async Task<Branch> AddBranchAsync(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task<bool> UpdateBranchAsync(Branch branch)
        {
            branch.UpdatedAt = DateTime.UtcNow;
            _context.Branches.Update(branch);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Branches.AsQueryable();
            
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.BranchID != excludeId.Value);
            }
            
            return await query.AnyAsync(b => b.BranchName.ToLower() == name.Trim().ToLower());
        }
    }
}
