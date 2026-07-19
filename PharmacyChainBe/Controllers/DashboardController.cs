using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;
using System.Security.Claims;

namespace PharmacyChainBe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "OperationsManager,Operations Manager,BranchManager,Branch Manager")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary(
            [FromQuery] int? branchId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            
            int? currentUserBranchId = null;
            var branchIdStr = User.FindFirst("BranchID")?.Value;
            if (!string.IsNullOrEmpty(branchIdStr) && int.TryParse(branchIdStr, out int bId))
            {
                currentUserBranchId = bId;
            }

            var result = await _dashboardService.GetDashboardSummaryAsync(
                branchId, 
                startDate, 
                endDate, 
                currentUserBranchId, 
                role);

            return Ok(new BaseApiResponse<DashboardSummaryDto>
            {
                Success = true,
                Message = "Tải thông tin Dashboard thành công.",
                Data = result
            });
        }
    }
}
