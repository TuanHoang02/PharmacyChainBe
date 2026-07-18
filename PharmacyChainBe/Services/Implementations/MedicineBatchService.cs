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
    public class MedicineBatchService : IMedicineBatchService
    {
        private readonly IMedicineBatchRepository _medicineBatchRepository;
        private readonly IAuthRepository _authRepository;

        public MedicineBatchService(IMedicineBatchRepository medicineBatchRepository, IAuthRepository authRepository)
        {
            _medicineBatchRepository = medicineBatchRepository;
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

        public async Task<PagedResponse<List<MedicineBatchDto>>> GetPagedAsync(int userId, MedicineBatchQuery query, CancellationToken cancellationToken = default)
        {
            int supplierId = await GetSupplierIdOrThrowAsync(userId);

            var paged = await _medicineBatchRepository.GetPagedAsync(supplierId, query, cancellationToken);
            var mappedList = paged.Data.Select(MapToDto).ToList();

            return new PagedResponse<List<MedicineBatchDto>>
            {
                Data = mappedList,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalRecords = paged.TotalRecords
            };
        }

        public async Task<MedicineBatchDetailDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
        {
            int supplierId = await GetSupplierIdOrThrowAsync(userId);

            var batch = await _medicineBatchRepository.GetByIdAsync(id, cancellationToken);
            if (batch == null)
            {
                throw new ApiException("Không tìm thấy thông tin lô thuốc.", 404);
            }

            if (batch.SupplierID != supplierId)
            {
                throw new ApiException("Lô thuốc không thuộc về nhà cung cấp này.", 403);
            }

            return MapToDetailDto(batch);
        }

        public async Task<MedicineBatchDetailDto> CreateAsync(int userId, CreateMedicineBatchRequest request, CancellationToken cancellationToken = default)
        {
            int supplierId = await GetSupplierIdOrThrowAsync(userId);

            // Trim fields
            request.BatchNumber = request.BatchNumber?.Trim() ?? string.Empty;

            // Validations
            if (string.IsNullOrWhiteSpace(request.BatchNumber) || request.BatchNumber.Length > 50)
            {
                throw new ApiException("Số lô sản xuất không hợp lệ.", 400);
            }

            if (request.ManufacturingDate > DateTime.UtcNow)
            {
                throw new ApiException("Ngày sản xuất không hợp lệ.", 400);
            }

            if (request.ExpiryDate <= request.ManufacturingDate)
            {
                throw new ApiException("Ngày hết hạn phải lớn hơn ngày sản xuất.", 400);
            }

            if (request.ReceivedQuantity <= 0)
            {
                throw new ApiException("Số lượng nhận phải lớn hơn 0.", 400);
            }

            // Existential checks
            var detail = await _medicineBatchRepository.GetPurchaseOrderDetailWithOrderAsync(request.PurchaseOrderDetailID, cancellationToken);
            if (detail == null || detail.PurchaseOrder == null)
            {
                throw new ApiException("Chi tiết đơn đặt hàng không tồn tại.", 404);
            }

            if (detail.PurchaseOrder.SupplierID != supplierId)
            {
                throw new ApiException("Đơn đặt hàng không thuộc về nhà cung cấp này.", 403);
            }

            if (detail.PurchaseOrder.OrderStatus != OrderStatus.Accepted)
            {
                throw new ApiException("Đơn đặt hàng chưa được chấp nhận.", 400);
            }

            if (detail.PurchaseOrder.DeliveryStatus != DeliveryStatus.Delivered)
            {
                throw new ApiException("Đơn đặt hàng chưa được giao.", 400);
            }

            // Check duplicate BatchNumber
            var batchExists = await _medicineBatchRepository.BatchNumberExistsAsync(request.BatchNumber, cancellationToken);
            if (batchExists)
            {
                throw new ApiException("Số lô sản xuất đã tồn tại.", 400);
            }

            // Map and populate fields
            var batch = new MedicineBatch
            {
                BatchNumber = request.BatchNumber,
                MedicineID = detail.MedicineID,
                BranchID = detail.PurchaseOrder.BranchID,
                SupplierID = supplierId,
                PurchaseOrderDetailID = request.PurchaseOrderDetailID,
                ManufacturingDate = request.ManufacturingDate,
                ExpiryDate = request.ExpiryDate,
                ReceivedQuantity = request.ReceivedQuantity,
                RemainingQuantity = request.ReceivedQuantity,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _medicineBatchRepository.CreateAsync(batch, cancellationToken);

            // Re-fetch detail DTO
            var result = await _medicineBatchRepository.GetByIdAsync(created.MedicineBatchID, cancellationToken);
            if (result == null)
            {
                throw new ApiException("Có lỗi xảy ra khi tạo lô thuốc.", 500);
            }

            return MapToDetailDto(result);
        }

        #region Helper Mappings

        private MedicineBatchDto MapToDto(MedicineBatch batch)
        {
            return new MedicineBatchDto
            {
                MedicineBatchID = batch.MedicineBatchID,
                BatchNumber = batch.BatchNumber,
                MedicineName = batch.Medicine?.MedicineName ?? string.Empty,
                PurchaseOrderCode = batch.PurchaseOrderDetail?.PurchaseOrder?.PurchaseOrderCode ?? string.Empty,
                BranchName = batch.Branch?.BranchName ?? string.Empty,
                ManufacturingDate = batch.ManufacturingDate,
                ExpiryDate = batch.ExpiryDate,
                ReceivedQuantity = batch.ReceivedQuantity,
                RemainingQuantity = batch.RemainingQuantity,
                CreatedAt = batch.CreatedAt
            };
        }

        private MedicineBatchDetailDto MapToDetailDto(MedicineBatch batch)
        {
            return new MedicineBatchDetailDto
            {
                MedicineBatchID = batch.MedicineBatchID,
                BatchNumber = batch.BatchNumber,
                MedicineID = batch.MedicineID,
                MedicineName = batch.Medicine?.MedicineName ?? string.Empty,
                SupplierName = batch.Supplier?.SupplierName ?? string.Empty,
                BranchName = batch.Branch?.BranchName ?? string.Empty,
                PurchaseOrderCode = batch.PurchaseOrderDetail?.PurchaseOrder?.PurchaseOrderCode ?? string.Empty,
                ManufacturingDate = batch.ManufacturingDate,
                ExpiryDate = batch.ExpiryDate,
                ReceivedQuantity = batch.ReceivedQuantity,
                RemainingQuantity = batch.RemainingQuantity,
                CreatedAt = batch.CreatedAt
            };
        }

        #endregion
    }
}
