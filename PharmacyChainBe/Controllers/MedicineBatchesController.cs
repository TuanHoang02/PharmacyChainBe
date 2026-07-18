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
    public class MedicineBatchesController : ControllerBase
    {
        private readonly IMedicineBatchService _medicineBatchService;

        public MedicineBatchesController(IMedicineBatchService medicineBatchService)
        {
            _medicineBatchService = medicineBatchService;
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
        public async Task<ActionResult<BaseApiResponse<PagedResponse<List<MedicineBatchDto>>>>> GetPaged([FromQuery] MedicineBatchQuery query, CancellationToken cancellationToken)
        {
            int userId = GetUserIdOrThrow();
            var data = await _medicineBatchService.GetPagedAsync(userId, query, cancellationToken);
            return Ok(new BaseApiResponse<PagedResponse<List<MedicineBatchDto>>>
            {
                Success = true,
                Message = "Lấy danh sách lô thuốc thành công.",
                Data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseApiResponse<MedicineBatchDetailDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            int userId = GetUserIdOrThrow();
            var data = await _medicineBatchService.GetByIdAsync(userId, id, cancellationToken);
            return Ok(new BaseApiResponse<MedicineBatchDetailDto>
            {
                Success = true,
                Message = "Lấy chi tiết lô thuốc thành công.",
                Data = data
            });
        }

        [HttpPost]
        public async Task<ActionResult<BaseApiResponse<MedicineBatchDetailDto>>> Create([FromBody] CreateMedicineBatchRequest request, CancellationToken cancellationToken)
        {
            int userId = GetUserIdOrThrow();
            var data = await _medicineBatchService.CreateAsync(userId, request, cancellationToken);
            return Ok(new BaseApiResponse<MedicineBatchDetailDto>
            {
                Success = true,
                Message = "Tạo lô thuốc thành công.",
                Data = data
            });
        }
    }
}
