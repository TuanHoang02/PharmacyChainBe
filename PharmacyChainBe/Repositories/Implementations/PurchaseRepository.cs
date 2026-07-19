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

        public async Task<(IEnumerable<PurchaseRequest> Data, int TotalRecords)> GetPagedPurchaseRequestsByBranchAsync(int branchId, int pageNumber, int pageSize)
        {
            var query = _context.PurchaseRequests
                .Where(pr => pr.BranchID == branchId)
                .Include(pr => pr.PurchaseRequestDetails)
                    .ThenInclude(prd => prd.Medicine)
                .Include(pr => pr.PurchaseOrders)
                    .ThenInclude(po => po.Supplier)
                .Include(pr => pr.PurchaseOrders)
                    .ThenInclude(po => po.PurchaseOrderDetails)
                        .ThenInclude(pod => pod.Medicine)
                .Include(pr => pr.PurchaseOrders)
                    .ThenInclude(po => po.PurchaseOrderDetails)
                        .ThenInclude(pod => pod.MedicineBatches)
                .Where(pr => pr.BranchID == branchId)
                .OrderByDescending(pr => pr.CreatedAt);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (data, totalRecords);
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

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int purchaseOrderId)
        {
            return await _context.PurchaseOrders
                .Include(po => po.PurchaseRequest)
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(pod => pod.Medicine)
                .FirstOrDefaultAsync(po => po.PurchaseOrderID == purchaseOrderId);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByRequestIdAsync(int purchaseRequestId)
        {
            return await _context.PurchaseOrders
                .Where(po => po.PurchaseRequestID == purchaseRequestId)
                .ToListAsync();
        }

        public async Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            _context.PurchaseOrders.Update(purchaseOrder);
            await _context.SaveChangesAsync();
        }

        public async Task<MedicineBatch?> GetMedicineBatchByIdAsync(int medicineBatchId)
        {
            return await _context.MedicineBatches
                .Include(mb => mb.PurchaseOrderDetail)
                    .ThenInclude(pod => pod!.Medicine)
                .FirstOrDefaultAsync(mb => mb.MedicineBatchID == medicineBatchId);
        }
    }
}
