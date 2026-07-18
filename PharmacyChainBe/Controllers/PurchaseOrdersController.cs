using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Supplier")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _service;

        public PurchaseOrdersController(IPurchaseOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? branchId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? status = null)
        {
            var supplierId = GetSupplierIdFromClaim();
            if (supplierId == null)
            {
                throw new ApiException("Tài khoản không liên kết với nhà cung cấp.", 403);
            }

            OrderStatus? statusEnum = null;
            if (status.HasValue)
            {
                if (!Enum.IsDefined(typeof(OrderStatus), status.Value))
                {
                    throw new ApiException("Giá trị trạng thái không hợp lệ.", 400);
                }
                statusEnum = (OrderStatus)status.Value;
            }

            var page = await _service.GetPagedAsync(
                supplierId.Value, pageNumber, pageSize,
                search, branchId, startDate, endDate, statusEnum);

            return Ok(new BaseApiResponse<PagedResponse<List<PurchaseOrderListItemDto>>>
            {
                Success = true,
                Message = "Lấy danh sách đơn mua thành công.",
                Data = page
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplierId = GetSupplierIdFromClaim();
            if (supplierId == null)
            {
                throw new ApiException("Tài khoản không liên kết với nhà cung cấp.", 403);
            }

            var detail = await _service.GetDetailAsync(id, supplierId.Value);
            if (detail == null)
            {
                throw new ApiException("Không tìm thấy đơn mua.", 404);
            }

            return Ok(new BaseApiResponse<PurchaseOrderDetailDto>
            {
                Success = true,
                Message = "Lấy chi tiết đơn mua thành công.",
                Data = detail
            });
        }

        [HttpPost("{id:int}/accept")]
        public async Task<IActionResult> Accept(int id)
        {
            var supplierId = GetSupplierIdFromClaim();
            if (supplierId == null)
            {
                throw new ApiException("Tài khoản không liên kết với nhà cung cấp.", 403);
            }

            var response = await _service.AcceptAsync(id, supplierId.Value);
            return Ok(new BaseApiResponse<SupplierResponseDto>
            {
                Success = true,
                Message = "Đơn mua đã được chấp nhận.",
                Data = response
            });
        }

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] SupplierResponseRequestDto request)
        {
            var supplierId = GetSupplierIdFromClaim();
            if (supplierId == null)
            {
                throw new ApiException("Tài khoản không liên kết với nhà cung cấp.", 403);
            }

            var response = await _service.RejectAsync(id, supplierId.Value, request ?? new SupplierResponseRequestDto());
            return Ok(new BaseApiResponse<SupplierResponseDto>
            {
                Success = true,
                Message = "Đơn mua đã bị từ chối.",
                Data = response
            });
        }

        [HttpPatch("{id:int}/delivery-status")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, [FromBody] UpdateDeliveryStatusRequest request)
        {
            var supplierId = GetSupplierIdFromClaim();
            if (supplierId == null)
            {
                throw new ApiException("Tài khoản không liên kết với nhà cung cấp.", 403);
            }

            var detail = await _service.UpdateDeliveryStatusAsync(id, supplierId.Value, request);
            if (detail == null)
            {
                throw new ApiException("Không tìm thấy đơn mua.", 404);
            }

            return Ok(new BaseApiResponse<PurchaseOrderDetailDto>
            {
                Success = true,
                Message = "Cập nhật trạng thái giao hàng thành công.",
                Data = detail
            });
        }

        private int? GetSupplierIdFromClaim()
        {
            var supplierClaim = User.FindFirst("SupplierID")?.Value;
            if (string.IsNullOrEmpty(supplierClaim) || !int.TryParse(supplierClaim, out var id))
            {
                return null;
            }
            return id;
        }
    }
}
