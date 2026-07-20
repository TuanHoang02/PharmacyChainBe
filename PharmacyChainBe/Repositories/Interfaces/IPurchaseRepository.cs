using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<PurchaseRequest> CreatePurchaseRequestAsync(PurchaseRequest request);
        Task<bool> HasPendingPurchaseRequestForMedicineAsync(int branchId, int medicineId);
        Task<PurchaseRequest?> GetPurchaseRequestByIdAsync(int id);
        Task<(IEnumerable<PurchaseRequest> Data, int TotalRecords)> GetPagedPurchaseRequestsByBranchAsync(int branchId, int pageNumber, int pageSize);
        Task<IEnumerable<MedicineBatch>> GetBatchesForPurchaseRequestAsync(int purchaseRequestId);
        Task UpdatePurchaseRequestAsync(PurchaseRequest request);
        Task AddMedicineBatchesAsync(IEnumerable<MedicineBatch> batches);
        Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int purchaseOrderId);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByRequestIdAsync(int purchaseRequestId);
        Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<MedicineBatch?> GetMedicineBatchByIdAsync(int medicineBatchId);
    }
}
