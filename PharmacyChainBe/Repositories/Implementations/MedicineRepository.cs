using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly AppDbContext _context;

        public MedicineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Medicine?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Medicines
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.MedicineID == id, cancellationToken);
        }

        public async Task<Medicine?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var trimmedName = name.Trim().ToLower();
            return await _context.Medicines
                .FirstOrDefaultAsync(m => m.MedicineName.ToLower() == trimmedName, cancellationToken);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.CategoryID == categoryId && c.IsActive, cancellationToken);
        }

        public async Task<PagedResponse<List<Medicine>>> GetPagedAsync(MedicineQuery query, CancellationToken cancellationToken = default)
        {
            IQueryable<Medicine> dbQuery = _context.Medicines.AsQueryable();

            // Search by MedicineName or GenericName
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var searchPattern = query.Search.Trim().ToLower();
                dbQuery = dbQuery.Where(m => m.MedicineName.ToLower().Contains(searchPattern)
                    || (m.GenericName != null && m.GenericName.ToLower().Contains(searchPattern)));
            }

            // Filter by Category
            if (query.CategoryId.HasValue)
            {
                dbQuery = dbQuery.Where(m => m.CategoryID == query.CategoryId.Value);
            }

            // Filter by IsActive
            if (query.IsActive.HasValue)
            {
                dbQuery = dbQuery.Where(m => m.IsActive == query.IsActive.Value);
            }

            // Sorting
            bool isDesc = query.IsDescending;
            string sortBy = (query.SortBy ?? string.Empty).Trim().ToLower();

            if (sortBy == "sellingprice" || sortBy == "price")
            {
                dbQuery = isDesc ? dbQuery.OrderByDescending(m => m.SellingPrice) : dbQuery.OrderBy(m => m.SellingPrice);
            }
            else
            {
                dbQuery = isDesc ? dbQuery.OrderByDescending(m => m.MedicineName) : dbQuery.OrderBy(m => m.MedicineName);
            }

            int totalRecords = await dbQuery.CountAsync(cancellationToken);
            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;

            var data = await dbQuery
                .Include(m => m.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<List<Medicine>>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<Medicine> AddAsync(Medicine medicine, CancellationToken cancellationToken = default)
        {
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync(cancellationToken);
            return medicine;
        }

        public async Task<bool> UpdateAsync(Medicine medicine, CancellationToken cancellationToken = default)
        {
            _context.Medicines.Update(medicine);
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
