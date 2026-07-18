using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Enums;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PagedResponse<List<PurchaseOrderListItemDto>>> GetPagedAsync(
            int supplierId,
            int pageNumber,
            int pageSize,
            string? search,
            int? branchId,
            DateTime? startDate,
            DateTime? endDate,
            OrderStatus? status);

        Task<PurchaseOrderDetailDto?> GetDetailAsync(int purchaseOrderId, int supplierId);

        Task<SupplierResponseDto> AcceptAsync(int purchaseOrderId, int supplierId);

        Task<SupplierResponseDto> RejectAsync(int purchaseOrderId, int supplierId, SupplierResponseRequestDto request);

        Task<PurchaseOrderDetailDto?> UpdateDeliveryStatusAsync(int purchaseOrderId, int supplierId, UpdateDeliveryStatusRequest request);
    }
}
