using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;

        public PurchaseOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<PurchaseOrder>>> GetPagedAsync(int supplierId, PurchaseOrderQuery query, CancellationToken cancellationToken = default)
        {
            IQueryable<PurchaseOrder> dbQuery = _context.PurchaseOrders.AsNoTracking();

            // Filter by SupplierID
            dbQuery = dbQuery.Where(po => po.SupplierID == supplierId);

            // Filter by OrderStatus
            if (query.OrderStatus.HasValue)
            {
                dbQuery = dbQuery.Where(po => po.OrderStatus == query.OrderStatus.Value);
            }

            // Filter by DeliveryStatus
            if (query.DeliveryStatus.HasValue)
            {
                dbQuery = dbQuery.Where(po => po.DeliveryStatus == query.DeliveryStatus.Value);
            }

            // Sorting
            bool isDesc = query.IsDescending;
            string sortBy = (query.SortBy ?? string.Empty).Trim().ToLower();

            if (sortBy == "expecteddeliverydate" || sortBy == "expecteddate")
            {
                dbQuery = isDesc ? dbQuery.OrderByDescending(po => po.ExpectedDeliveryDate) : dbQuery.OrderBy(po => po.ExpectedDeliveryDate);
            }
            else
            {
                // Default to CreatedAt descending
                dbQuery = isDesc ? dbQuery.OrderByDescending(po => po.CreatedAt) : dbQuery.OrderBy(po => po.CreatedAt);
            }

            int totalRecords = await dbQuery.CountAsync(cancellationToken);
            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;

            var data = await dbQuery
                .Include(po => po.Branch)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<List<PurchaseOrder>>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Branch)
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(pod => pod.Medicine)
                .FirstOrDefaultAsync(po => po.PurchaseOrderID == id, cancellationToken);
        }

        public async Task<bool> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
        {
            _context.PurchaseOrders.Update(purchaseOrder);
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
