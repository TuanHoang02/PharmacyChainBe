using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetSuppliersAsync(
            string? searchTerm, 
            bool? isActive, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(s => s.SupplierName.ToLower().Contains(search) 
                                      || (s.ContactName != null && s.ContactName.ToLower().Contains(search))
                                      || (s.Email != null && s.Email.ToLower().Contains(search))
                                      || (s.PhoneNumber != null && s.PhoneNumber.Contains(search)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == id);
        }

        public async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            supplier.UpdatedAt = DateTime.UtcNow;
            _context.Suppliers.Update(supplier);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Suppliers.AsQueryable();

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.SupplierID != excludeId.Value);
            }

            return await query.AnyAsync(s => s.SupplierName.ToLower() == name.Trim().ToLower());
        }
    }
}
