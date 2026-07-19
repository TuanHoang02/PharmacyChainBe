using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Inventory> Data, int TotalRecords)> GetInventoriesAsync(int branchId, InventoryRequestDto request)
        {
            var query = _context.Inventories
                .Include(i => i.Medicine)
                    .ThenInclude(m => m!.Category)
                .Where(i => i.BranchID == branchId)
                .AsQueryable();

            if (request.CategoryId.HasValue)
            {
                query = query.Where(i => i.Medicine != null && i.Medicine.CategoryID == request.CategoryId.Value);
            }

            if (!string.IsNullOrEmpty(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.ToLower();
                query = query.Where(i => i.Medicine != null && 
                    (i.Medicine.MedicineName.ToLower().Contains(keyword) || 
                     (i.Medicine.GenericName != null && i.Medicine.GenericName.ToLower().Contains(keyword))));
            }

            if (request.IsLowStock.HasValue && request.IsLowStock.Value)
            {
                query = query.Where(i => i.QuantityInStock <= i.ReorderLevel);
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderBy(i => i.Medicine != null ? i.Medicine.MedicineName : "")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<Inventory?> GetInventoryAsync(int branchId, int medicineId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.BranchID == branchId && i.MedicineID == medicineId);
        }

        public async Task AddStockAsync(int branchId, int medicineId, int quantity)
        {
            var inventory = await GetInventoryAsync(branchId, medicineId);
            if (inventory == null)
            {
                inventory = new Inventory
                {
                    BranchID = branchId,
                    MedicineID = medicineId,
                    QuantityInStock = quantity,
                    ReorderLevel = 10, // Default fallback
                    LastUpdatedAt = DateTime.UtcNow
                };
                _context.Inventories.Add(inventory);
            }
            else
            {
                inventory.QuantityInStock += quantity;
                inventory.LastUpdatedAt = DateTime.UtcNow;
                _context.Inventories.Update(inventory);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeductStockAsync(int branchId, int medicineId, int quantity)
        {
            var inventory = await GetInventoryAsync(branchId, medicineId);
            if (inventory == null || inventory.QuantityInStock < quantity)
            {
                throw new Exception("Not enough stock to deduct.");
            }
            
            inventory.QuantityInStock -= quantity;
            inventory.LastUpdatedAt = DateTime.UtcNow;
            _context.Inventories.Update(inventory);
            
            await _context.SaveChangesAsync();
        }
    }
}
