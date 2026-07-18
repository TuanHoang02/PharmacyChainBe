using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IAuthRepository _authRepository;

        public PurchaseOrderService(IPurchaseOrderRepository purchaseOrderRepository, IAuthRepository authRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _authRepository = authRepository;
        }

        private async Task<int> GetSupplierIdOrThrowAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null || !user.SupplierID.HasValue)
            {
                throw new ApiException("Người dùng không thuộc về nhà cung cấp nào.", 403);
            }
            return user.SupplierID.Value;
        }

        public async Task<PagedResponse<List<PurchaseOrderDto>>> GetPagedAsync(int userId, PurchaseOrderQuery query, CancellationToken cancellationToken = default)
        {
            int supplierId = await GetSupplierIdOrThrowAsync(userId);

            var paged = await _purchaseOrderRepository.GetPagedAsync(supplierId, query, cancellationToken);
            var mappedList = paged.Data.Select(MapToDto).ToList();

            return new PagedResponse<List<PurchaseOrderDto>>
            {
                Data = mappedList,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalRecords = paged.TotalRecords
            };
        }

        public async Task<PurchaseOrderDetailDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
        {
            int supplierId = await GetSupplierIdOrThrowAsync(userId);

            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken);
            if (purchaseOrder == null)
            {
                throw new ApiException("Không tìm thấy đơn đặt hàng.", 404);
            }

            if (purchaseOrder.SupplierID != supplierId)
            {
                throw new ApiException("Đơn đặt hàng không thuộc về nhà cung cấp này.", 403);
            }

            return MapToDetailDto(purchaseOrder);
        }

        public async Task<bool> UpdateDeliveryStatusAsync(int userId, int id, UpdateDeliveryStatusRequest request, CancellationToken cancellationToken = default)
        {
            int supplierId = await GetSupplierIdOrThrowAsync(userId);

            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken);
            if (purchaseOrder == null)
            {
                throw new ApiException("Không tìm thấy đơn đặt hàng.", 404);
            }

            if (purchaseOrder.SupplierID != supplierId)
            {
                throw new ApiException("Đơn đặt hàng không thuộc về nhà cung cấp này.", 403);
            }

            if (purchaseOrder.OrderStatus != OrderStatus.Accepted)
            {
                throw new ApiException("Đơn đặt hàng chưa được chấp nhận.", 400);
            }

            if (request.DeliveryStatus == DeliveryStatus.Received)
            {
                throw new ApiException("Nhà cung cấp không được phép cập nhật trạng thái đã nhận hàng.", 400);
            }

            // State transition validation (allow same status to update note, but check progress transitions)
            if (purchaseOrder.DeliveryStatus != request.DeliveryStatus)
            {
                bool isValidTransition = false;

                if (purchaseOrder.DeliveryStatus == DeliveryStatus.NotStarted && request.DeliveryStatus == DeliveryStatus.Preparing)
                {
                    isValidTransition = true;
                }
                else if (purchaseOrder.DeliveryStatus == DeliveryStatus.Preparing && request.DeliveryStatus == DeliveryStatus.Shipping)
                {
                    isValidTransition = true;
                }
                else if (purchaseOrder.DeliveryStatus == DeliveryStatus.Shipping && request.DeliveryStatus == DeliveryStatus.Delivered)
                {
                    isValidTransition = true;
                }

                if (!isValidTransition)
                {
                    throw new ApiException("Trạng thái giao hàng chuyển đổi không hợp lệ.", 400);
                }
            }

            // Set DeliveredAt automatically
            if (request.DeliveryStatus == DeliveryStatus.Delivered)
            {
                purchaseOrder.DeliveredAt ??= DateTime.UtcNow;
            }

            purchaseOrder.DeliveryStatus = request.DeliveryStatus;
            purchaseOrder.SupplierResponseNote = request.SupplierResponseNote?.Trim();

            return await _purchaseOrderRepository.UpdateAsync(purchaseOrder, cancellationToken);
        }

        #region Helper Mappings

        private PurchaseOrderDto MapToDto(PurchaseOrder po)
        {
            return new PurchaseOrderDto
            {
                PurchaseOrderID = po.PurchaseOrderID,
                PurchaseOrderCode = po.PurchaseOrderCode,
                BranchName = po.Branch?.BranchName ?? string.Empty,
                TotalAmount = po.TotalAmount,
                OrderStatus = po.OrderStatus,
                DeliveryStatus = po.DeliveryStatus,
                CreatedAt = po.CreatedAt,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                DeliveredAt = po.DeliveredAt
            };
        }

        private PurchaseOrderDetailDto MapToDetailDto(PurchaseOrder po)
        {
            return new PurchaseOrderDetailDto
            {
                PurchaseOrderID = po.PurchaseOrderID,
                PurchaseOrderCode = po.PurchaseOrderCode,
                BranchName = po.Branch?.BranchName ?? string.Empty,
                SupplierName = po.Supplier?.SupplierName ?? string.Empty,
                OrderStatus = po.OrderStatus,
                DeliveryStatus = po.DeliveryStatus,
                TotalAmount = po.TotalAmount,
                SupplierResponseNote = po.SupplierResponseNote,
                CreatedAt = po.CreatedAt,
                ConfirmedAt = po.ConfirmedAt,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                DeliveredAt = po.DeliveredAt,
                ReceivedAt = po.ReceivedAt,
                Items = po.PurchaseOrderDetails?.Select(MapToItemDto).ToList() ?? new List<PurchaseOrderItemDto>()
            };
        }

        private PurchaseOrderItemDto MapToItemDto(PurchaseOrderDetail pod)
        {
            return new PurchaseOrderItemDto
            {
                MedicineID = pod.MedicineID,
                MedicineName = pod.Medicine?.MedicineName ?? string.Empty,
                OrderedQuantity = pod.OrderedQuantity,
                ReceivedQuantity = pod.ReceivedQuantity,
                UnitPrice = pod.UnitPrice,
                LineTotal = pod.LineTotal
            };
        }

        #endregion
    }
}
