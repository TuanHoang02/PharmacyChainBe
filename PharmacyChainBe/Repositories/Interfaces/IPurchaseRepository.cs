using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<PurchaseRequest> CreatePurchaseRequestAsync(PurchaseRequest request);
        Task<bool> HasPendingPurchaseRequestForMedicineAsync(int branchId, int medicineId);
        Task<PurchaseRequest?> GetPurchaseRequestByIdAsync(int id);
        Task UpdatePurchaseRequestAsync(PurchaseRequest request);
        Task AddMedicineBatchesAsync(IEnumerable<MedicineBatch> batches);
    }
}
