using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class MedicineBatchRepository : IMedicineBatchRepository
    {
        private readonly AppDbContext _context;

        public MedicineBatchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<MedicineBatch>>> GetPagedAsync(int supplierId, MedicineBatchQuery query, CancellationToken cancellationToken = default)
        {
            IQueryable<MedicineBatch> dbQuery = _context.MedicineBatches.AsNoTracking();

            // Scope by SupplierID
            dbQuery = dbQuery.Where(b => b.SupplierID == supplierId);

            // Filter by MedicineID
            if (query.MedicineID.HasValue)
            {
                dbQuery = dbQuery.Where(b => b.MedicineID == query.MedicineID.Value);
            }

            // Filter by PurchaseOrderID
            if (query.PurchaseOrderID.HasValue)
            {
                dbQuery = dbQuery.Where(b => b.PurchaseOrderDetail != null && b.PurchaseOrderDetail.PurchaseOrderID == query.PurchaseOrderID.Value);
            }

            // Filter by BatchNumber
            if (!string.IsNullOrWhiteSpace(query.BatchNumber))
            {
                var batchPattern = query.BatchNumber.Trim().ToLower();
                dbQuery = dbQuery.Where(b => b.BatchNumber.ToLower().Contains(batchPattern));
            }

            // Sorting
            bool isDesc = query.IsDescending;
            string sortBy = (query.SortBy ?? string.Empty).Trim().ToLower();

            if (sortBy == "manufacturingdate" || sortBy == "manufacturing")
            {
                dbQuery = isDesc ? dbQuery.OrderByDescending(b => b.ManufacturingDate) : dbQuery.OrderBy(b => b.ManufacturingDate);
            }
            else if (sortBy == "expirydate" || sortBy == "expiry")
            {
                dbQuery = isDesc ? dbQuery.OrderByDescending(b => b.ExpiryDate) : dbQuery.OrderBy(b => b.ExpiryDate);
            }
            else
            {
                // Default to CreatedAt descending
                dbQuery = isDesc ? dbQuery.OrderByDescending(b => b.CreatedAt) : dbQuery.OrderBy(b => b.CreatedAt);
            }

            int totalRecords = await dbQuery.CountAsync(cancellationToken);
            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;

            var data = await dbQuery
                .Include(b => b.Medicine)
                .Include(b => b.Branch)
                .Include(b => b.PurchaseOrderDetail)
                    .ThenInclude(d => d!.PurchaseOrder)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<List<MedicineBatch>>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<MedicineBatch?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.MedicineBatches
                .AsNoTracking()
                .Include(b => b.Medicine)
                .Include(b => b.Supplier)
                .Include(b => b.Branch)
                .Include(b => b.PurchaseOrderDetail)
                    .ThenInclude(d => d!.PurchaseOrder)
                .FirstOrDefaultAsync(b => b.MedicineBatchID == id, cancellationToken);
        }

        public async Task<bool> BatchNumberExistsAsync(string batchNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(batchNumber)) return false;
            var trimmed = batchNumber.Trim().ToLower();
            return await _context.MedicineBatches
                .AnyAsync(b => b.BatchNumber.ToLower() == trimmed, cancellationToken);
        }

        public async Task<MedicineBatch> CreateAsync(MedicineBatch batch, CancellationToken cancellationToken = default)
        {
            _context.MedicineBatches.Add(batch);
            await _context.SaveChangesAsync(cancellationToken);
            return batch;
        }

        public async Task<PurchaseOrderDetail?> GetPurchaseOrderDetailWithOrderAsync(int detailId, CancellationToken cancellationToken = default)
        {
            return await _context.PurchaseOrderDetails
                .Include(d => d.PurchaseOrder)
                .FirstOrDefaultAsync(d => d.PurchaseOrderDetailID == detailId, cancellationToken);
        }
    }
}
