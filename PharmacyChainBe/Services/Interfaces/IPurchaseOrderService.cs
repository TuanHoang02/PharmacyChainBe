using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PagedResponse<List<PurchaseOrderDto>>> GetPagedAsync(int userId, PurchaseOrderQuery query, CancellationToken cancellationToken = default);
        Task<PurchaseOrderDetailDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
        Task<bool> UpdateDeliveryStatusAsync(int userId, int id, UpdateDeliveryStatusRequest request, CancellationToken cancellationToken = default);
    }
}
