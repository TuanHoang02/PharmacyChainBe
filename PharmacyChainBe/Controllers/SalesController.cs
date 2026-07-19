using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;
using PharmacyChainBe.Exceptions;
using System.Security.Claims;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Pharmacist")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        private (int BranchId, int UserId) GetUserClaims()
        {
            var branchIdClaim = User.FindFirst("BranchID")?.Value;
            if (string.IsNullOrEmpty(branchIdClaim) || !int.TryParse(branchIdClaim, out int branchId))
            {
                throw new ApiException("Người dùng không thuộc về chi nhánh nào.", 403);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new ApiException("Không xác định được ID người dùng.", 401);
            }

            return (branchId, userId);
        }

        [HttpPost("invoice")]
        public async Task<IActionResult> CreateSalesInvoice([FromBody] CreateSalesInvoiceDto request)
        {
            var claims = GetUserClaims();
            await _salesService.CreateSalesInvoiceAsync(claims.BranchId, claims.UserId, request);

            return Ok(new BaseApiResponse<object> 
            { 
                Success = true, 
                Message = "Tạo hóa đơn bán hàng và thanh toán thành công." 
            });
        }

        [HttpGet]
        public async Task<ActionResult<BaseApiResponse<PagedResponse<List<SalesHistoryDto>>>>> GetPaged([FromQuery] SalesHistoryQuery query, CancellationToken cancellationToken)
        {
            var data = await _salesService.GetPagedAsync(query, cancellationToken);
            return Ok(new BaseApiResponse<PagedResponse<List<SalesHistoryDto>>>
            {
                Success = true,
                Message = "Lấy lịch sử bán hàng thành công.",
                Data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseApiResponse<DTOs.Response.SalesInvoiceDetailDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _salesService.GetByIdAsync(id, cancellationToken);
            return Ok(new BaseApiResponse<DTOs.Response.SalesInvoiceDetailDto>
            {
                Success = true,
                Message = "Lấy chi tiết hóa đơn thành công.",
                Data = data
            });
        }
    }
}
