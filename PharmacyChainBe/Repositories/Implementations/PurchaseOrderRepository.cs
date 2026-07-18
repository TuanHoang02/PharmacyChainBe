using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.Enums;
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

        public async Task<PagedResponse<List<PurchaseOrder>>> GetBySupplierPagedAsync(
            int supplierId,
            int pageNumber,
            int pageSize,
            string? search,
            int? branchId,
            DateTime? startDate,
            DateTime? endDate,
            OrderStatus? status)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Branch)
                .Where(po => po.SupplierID == supplierId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmed = search.Trim();
                query = query.Where(po => po.PurchaseOrderCode.Contains(trimmed));
            }

            if (branchId.HasValue)
            {
                query = query.Where(po => po.BranchID == branchId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(po => po.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Include the entire endDate day.
                var endOfDay = endDate.Value.Date.AddDays(1);
                query = query.Where(po => po.CreatedAt < endOfDay);
            }

            if (status.HasValue)
            {
                query = query.Where(po => po.OrderStatus == status.Value);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(po => po.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<List<PurchaseOrder>>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<PurchaseOrder?> GetByIdAndSupplierAsync(int purchaseOrderId, int supplierId)
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Branch)
                .Include(po => po.CreatedByUser)
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(pod => pod.Medicine)
                .FirstOrDefaultAsync(po => po.PurchaseOrderID == purchaseOrderId && po.SupplierID == supplierId);
        }

        public async Task<PurchaseOrder?> GetByIdAndSupplierForUpdateAsync(int purchaseOrderId, int supplierId)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Branch)
                .FirstOrDefaultAsync(po => po.PurchaseOrderID == purchaseOrderId && po.SupplierID == supplierId);
        }

        public async Task UpdateAsync(PurchaseOrder purchaseOrder)
        {
            await _context.SaveChangesAsync();
        }
    }
}
