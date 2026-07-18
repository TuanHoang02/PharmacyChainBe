using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext _context;

        public PurchaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseRequest> CreatePurchaseRequestAsync(PurchaseRequest request)
        {
            _context.PurchaseRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> HasPendingPurchaseRequestForMedicineAsync(int branchId, int medicineId)
        {
            return await _context.PurchaseRequestDetails
                .AnyAsync(prd => prd.MedicineID == medicineId && 
                                 prd.PurchaseRequest != null && 
                                 prd.PurchaseRequest.BranchID == branchId && 
                                 prd.PurchaseRequest.Status == PurchaseRequestStatus.Pending);
        }

        public async Task<PurchaseRequest?> GetPurchaseRequestByIdAsync(int id)
        {
            return await _context.PurchaseRequests
                .Include(pr => pr.PurchaseRequestDetails)
                .FirstOrDefaultAsync(pr => pr.PurchaseRequestID == id);
        }

        public async Task<IEnumerable<PurchaseRequest>> GetPurchaseRequestsByBranchAsync(int branchId)
        {
            return await _context.PurchaseRequests
                .Include(pr => pr.PurchaseRequestDetails)
                    .ThenInclude(prd => prd.Medicine)
                .Where(pr => pr.BranchID == branchId)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicineBatch>> GetBatchesForPurchaseRequestAsync(int purchaseRequestId)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(pod => pod.MedicineBatches)
                .FirstOrDefaultAsync(po => po.PurchaseRequestID == purchaseRequestId);

            if (purchaseOrder == null)
            {
                return new List<MedicineBatch>();
            }

            return purchaseOrder.PurchaseOrderDetails.SelectMany(pod => pod.MedicineBatches).ToList();
        }

        public async Task UpdatePurchaseRequestAsync(PurchaseRequest request)
        {
            _context.PurchaseRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task AddMedicineBatchesAsync(IEnumerable<MedicineBatch> batches)
        {
            _context.MedicineBatches.AddRange(batches);
            await _context.SaveChangesAsync();
        }
    }
}
