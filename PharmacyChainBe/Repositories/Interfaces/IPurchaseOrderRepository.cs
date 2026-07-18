using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<PagedResponse<List<PurchaseOrder>>> GetPagedAsync(int supplierId, PurchaseOrderQuery query, CancellationToken cancellationToken = default);
        Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    }
}
