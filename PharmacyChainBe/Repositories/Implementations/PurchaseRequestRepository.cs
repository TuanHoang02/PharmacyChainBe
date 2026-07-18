using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PharmacyChainBe.DTOs.Request;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class PurchaseRequestRepository : IPurchaseRequestRepository
    {
        private readonly AppDbContext _context;

        public PurchaseRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<PurchaseRequest> Requests, int TotalRecords)> GetPurchaseRequestsAsync(PurchaseRequestFilter filter)
        {
            var query = _context.PurchaseRequests
                .Include(pr => pr.Branch)
                .Include(pr => pr.CreatedByUser)
                .AsQueryable();

            if (filter.Status.HasValue)
            {
                query = query.Where(pr => pr.Status == filter.Status.Value);
            }

            if (filter.BranchID.HasValue)
            {
                query = query.Where(pr => pr.BranchID == filter.BranchID.Value);
            }

            query = query.OrderByDescending(pr => pr.CreatedAt);

            var totalRecords = await query.CountAsync();

            var requests = await query
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return (requests, totalRecords);
        }

        public async Task<PurchaseRequest?> GetPurchaseRequestByIdAsync(int id)
        {
            return await _context.PurchaseRequests
                .Include(pr => pr.Branch)
                .Include(pr => pr.CreatedByUser)
                .Include(pr => pr.PurchaseRequestDetails)
                    .ThenInclude(prd => prd.Medicine)
                .Include(pr => pr.PurchaseOrders)
                    .ThenInclude(po => po.Supplier)
                .Include(pr => pr.PurchaseOrders)
                    .ThenInclude(po => po.PurchaseOrderDetails)
                        .ThenInclude(pod => pod.Medicine)
                .FirstOrDefaultAsync(pr => pr.PurchaseRequestID == id);
        }

        public async Task UpdatePurchaseRequestAsync(PurchaseRequest request)
        {
            _context.PurchaseRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersAsync()
        {
            return await _context.Suppliers.Where(s => s.IsActive).ToListAsync();
        }

        public async Task CreatePurchaseOrderAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}
