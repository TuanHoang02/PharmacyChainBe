using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Controllers
{
    [Route("api/operations-manager/branch-performance")]
    [ApiController]
    [Authorize(Roles = "Operations Manager")]
    public class BranchPerformanceController : ControllerBase
    {
        private readonly IBranchPerformanceService _performanceService;

        public BranchPerformanceController(IBranchPerformanceService performanceService)
        {
            _performanceService = performanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPerformanceData([FromQuery] int? branchId, [FromQuery] string period = "This Month", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var data = await _performanceService.GetPerformanceDataAsync(branchId, period, startDate, endDate);
                return Ok(new PharmacyChainBe.DTOs.BaseApiResponse<PharmacyChainBe.DTOs.Response.BranchPerformanceResponse>
                {
                    Success = true,
                    Message = "Branch performance data retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PharmacyChainBe.DTOs.BaseApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving branch performance data.",
                    Data = new { Error = ex.Message }
                });
            }
        }
    }
}
