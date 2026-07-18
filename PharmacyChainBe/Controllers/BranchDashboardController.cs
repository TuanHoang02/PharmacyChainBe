using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BranchDashboardController : ControllerBase
    {
        private readonly IBranchDashboardService _service;

        public BranchDashboardController(IBranchDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var branchId = GetBranchIdFromClaim();
            if (branchId == null)
            {
                throw new ApiException("Tài khoản không liên kết với chi nhánh.", 403);
            }

            var dashboard = await _service.GetDashboardAsync(branchId.Value);
            return Ok(new BaseApiResponse<BranchDashboardDto>
            {
                Success = true,
                Message = "Lấy dữ liệu dashboard thành công.",
                Data = dashboard,
            });
        }

        private int? GetBranchIdFromClaim()
        {
            var branchClaim = User.FindFirst("BranchID")?.Value;
            if (string.IsNullOrEmpty(branchClaim) || !int.TryParse(branchClaim, out var id))
            {
                return null;
            }
            return id;
        }
    }
}
