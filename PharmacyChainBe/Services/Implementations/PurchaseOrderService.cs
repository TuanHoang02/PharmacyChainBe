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
        private readonly IPurchaseOrderRepository _repository;

        public PurchaseOrderService(IPurchaseOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<List<PurchaseOrderListItemDto>>> GetPagedAsync(
            int supplierId,
            int pageNumber,
            int pageSize,
            string? search,
            int? branchId,
            DateTime? startDate,
            DateTime? endDate,
            OrderStatus? status)
        {
            if (startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date)
            {
                throw new ApiException("The Start Date cannot be later than the End Date.", 400);
            }

            var page = await _repository.GetBySupplierPagedAsync(
                supplierId, pageNumber, pageSize, search, branchId,
                startDate, endDate, status);

            var items = page.Data.Select(MapToListItem).ToList();

            return new PagedResponse<List<PurchaseOrderListItemDto>>
            {
                Data = items,
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalRecords = page.TotalRecords
            };
        }

        public async Task<PurchaseOrderDetailDto?> GetDetailAsync(int purchaseOrderId, int supplierId)
        {
            var po = await _repository.GetByIdAndSupplierAsync(purchaseOrderId, supplierId);
            if (po == null) return null;
            return MapToDetail(po);
        }

        public async Task<SupplierResponseDto> AcceptAsync(int purchaseOrderId, int supplierId)
        {
            var po = await LoadPendingOrderAsync(purchaseOrderId, supplierId);
            po.OrderStatus = OrderStatus.Accepted;
            po.ConfirmedAt = DateTime.UtcNow;
            po.SupplierResponseNote = null;
            await _repository.UpdateAsync(po);
            return MapToResponse(po, null);
        }

        public async Task<SupplierResponseDto> RejectAsync(int purchaseOrderId, int supplierId, SupplierResponseRequestDto request)
        {
            var reason = request?.RejectionReason?.Trim();
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ApiException("Vui lòng nhập lý do từ chối.", 400);
            }
            if (reason.Length > 255)
            {
                throw new ApiException("Lý do từ chối không được vượt quá 255 ký tự.", 400);
            }

            var po = await LoadPendingOrderAsync(purchaseOrderId, supplierId);
            po.OrderStatus = OrderStatus.Rejected;
            po.ConfirmedAt = DateTime.UtcNow;
            po.SupplierResponseNote = reason;
            await _repository.UpdateAsync(po);
            return MapToResponse(po, reason);
        }

        private async Task<PurchaseOrder> LoadPendingOrderAsync(int purchaseOrderId, int supplierId)
        {
            var po = await _repository.GetByIdAndSupplierForUpdateAsync(purchaseOrderId, supplierId);
            if (po == null)
            {
                throw new ApiException("Không tìm thấy đơn mua.", 404);
            }
            if (po.OrderStatus != OrderStatus.PendingSupplierConfirmation)
            {
                throw new ApiException("Đơn mua không ở trạng thái chờ xác nhận.", 409);
            }
            return po;
        }

        private static SupplierResponseDto MapToResponse(PurchaseOrder po, string? reason) => new()
        {
            PurchaseOrderID = po.PurchaseOrderID,
            OrderStatus = po.OrderStatus.ToString(),
            ConfirmedAt = po.ConfirmedAt ?? DateTime.UtcNow,
            RejectionReason = reason
        };

        private static PurchaseOrderListItemDto MapToListItem(PurchaseOrder po) => new()
        {
            PurchaseOrderID = po.PurchaseOrderID,
            PurchaseOrderCode = po.PurchaseOrderCode,
            BranchID = po.BranchID,
            BranchName = po.Branch?.BranchName ?? string.Empty,
            BranchAddress = po.Branch?.Address ?? string.Empty,
            OrderDate = po.CreatedAt,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            TotalAmount = po.TotalAmount,
            OrderStatus = po.OrderStatus.ToString(),
            DeliveryStatus = po.DeliveryStatus.ToString()
        };

        private static PurchaseOrderDetailDto MapToDetail(PurchaseOrder po) => new()
        {
            PurchaseOrderID = po.PurchaseOrderID,
            PurchaseOrderCode = po.PurchaseOrderCode,
            BranchID = po.BranchID,
            BranchName = po.Branch?.BranchName ?? string.Empty,
            BranchAddress = po.Branch?.Address ?? string.Empty,
            BranchPhoneNumber = po.Branch?.PhoneNumber,
            OrderDate = po.CreatedAt,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            ConfirmedAt = po.ConfirmedAt,
            TotalAmount = po.TotalAmount,
            OrderStatus = po.OrderStatus.ToString(),
            DeliveryStatus = po.DeliveryStatus.ToString(),
            Notes = po.SupplierResponseNote,
            CreatedByFullName = po.CreatedByUser?.FullName,
            Items = po.PurchaseOrderDetails
                .OrderBy(d => d.PurchaseOrderDetailID)
                .Select(d => new PurchaseOrderItemDto
                {
                    PurchaseOrderDetailID = d.PurchaseOrderDetailID,
                    MedicineID = d.MedicineID,
                    MedicineName = d.Medicine?.MedicineName ?? string.Empty,
                    Unit = d.Medicine?.Unit,
                    OrderedQuantity = d.OrderedQuantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.LineTotal
                })
                .ToList()
        };

        public async Task<PurchaseOrderDetailDto?> UpdateDeliveryStatusAsync(int purchaseOrderId, int supplierId, UpdateDeliveryStatusRequest request)
        {
            var po = await _repository.GetByIdAndSupplierForUpdateAsync(purchaseOrderId, supplierId);
            if (po == null)
            {
                throw new ApiException("Không tìm thấy đơn mua.", 404);
            }
            if (po.OrderStatus != OrderStatus.Accepted)
            {
                throw new ApiException("Đơn mua chưa được chấp nhận hoặc đã hoàn thành/hủy.", 400);
            }
            if (po.DeliveryStatus == DeliveryStatus.Received)
            {
                throw new ApiException("Không thể cập nhật trạng thái đơn mua đã nhận hàng.", 400);
            }

            po.DeliveryStatus = request.DeliveryStatus;
            po.SupplierResponseNote = request.SupplierResponseNote?.Trim();
            
            if (request.DeliveryStatus == DeliveryStatus.Delivered)
            {
                po.DeliveredAt = DateTime.UtcNow;
            }

            await _repository.UpdateAsync(po);
            return MapToDetail(po);
        }
    }
}
