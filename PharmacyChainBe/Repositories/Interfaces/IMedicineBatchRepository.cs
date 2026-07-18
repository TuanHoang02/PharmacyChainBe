using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IMedicineBatchRepository
    {
        Task<PagedResponse<List<MedicineBatch>>> GetPagedAsync(int supplierId, MedicineBatchQuery query, CancellationToken cancellationToken = default);
        Task<MedicineBatch?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> BatchNumberExistsAsync(string batchNumber, CancellationToken cancellationToken = default);
        Task<MedicineBatch> CreateAsync(MedicineBatch batch, CancellationToken cancellationToken = default);
        Task<PurchaseOrderDetail?> GetPurchaseOrderDetailWithOrderAsync(int detailId, CancellationToken cancellationToken = default);
    }
}
