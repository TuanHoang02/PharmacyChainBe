using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;
using System.Security.Claims;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Supplier")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        private int GetUserIdOrThrow()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                throw new UnauthorizedAccessException("Không thể xác thực người dùng.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<BaseApiResponse<PagedResponse<List<PurchaseOrderDto>>>>> GetPaged([FromQuery] PurchaseOrderQuery query, CancellationToken cancellationToken)
        {
            int userId = GetUserIdOrThrow();
            var data = await _purchaseOrderService.GetPagedAsync(userId, query, cancellationToken);
            return Ok(new BaseApiResponse<PagedResponse<List<PurchaseOrderDto>>>
            {
                Success = true,
                Message = "Lấy danh sách đơn đặt hàng thành công.",
                Data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseApiResponse<PurchaseOrderDetailDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            int userId = GetUserIdOrThrow();
            var data = await _purchaseOrderService.GetByIdAsync(userId, id, cancellationToken);
            return Ok(new BaseApiResponse<PurchaseOrderDetailDto>
            {
                Success = true,
                Message = "Lấy chi tiết đơn đặt hàng thành công.",
                Data = data
            });
        }

        [HttpPatch("{id}/delivery-status")]
        public async Task<ActionResult<BaseApiResponse<string>>> UpdateDeliveryStatus(int id, [FromBody] UpdateDeliveryStatusRequest request, CancellationToken cancellationToken)
        {
            int userId = GetUserIdOrThrow();
            await _purchaseOrderService.UpdateDeliveryStatusAsync(userId, id, request, cancellationToken);
            return Ok(new BaseApiResponse<string>
            {
                Success = true,
                Message = "Cập nhật trạng thái giao hàng thành công.",
                Data = "Cập nhật trạng thái giao hàng thành công."
            });
        }
    }
}
