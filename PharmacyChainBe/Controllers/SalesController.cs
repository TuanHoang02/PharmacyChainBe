using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;

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
        public async Task<ActionResult<BaseApiResponse<SalesInvoiceDetailDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _salesService.GetByIdAsync(id, cancellationToken);
            return Ok(new BaseApiResponse<SalesInvoiceDetailDto>
            {
                Success = true,
                Message = "Lấy chi tiết hóa đơn thành công.",
                Data = data
            });
        }
    }
}
