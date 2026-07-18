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
    public class BranchReportController : ControllerBase
    {
        private readonly IBranchReportService _service;

        public BranchReportController(IBranchReportService service)
        {
            _service = service;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            ValidateDateRange(startDate, endDate);

            var branchId = GetBranchIdFromClaim();
            if (branchId == null)
            {
                throw new ApiException("Tài khoản không liên kết với chi nhánh.", 403);
            }

            var report = await _service.GetSalesReportAsync(branchId.Value, startDate!.Value, endDate!.Value);
            return Ok(new BaseApiResponse<BranchReportResponseDto>
            {
                Success = true,
                Message = "Lấy báo cáo doanh thu thành công.",
                Data = report,
            });
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryReport()
        {
            var branchId = GetBranchIdFromClaim();
            if (branchId == null)
            {
                throw new ApiException("Tài khoản không liên kết với chi nhánh.", 403);
            }

            var report = await _service.GetInventoryReportAsync(branchId.Value);
            return Ok(new BaseApiResponse<BranchReportResponseDto>
            {
                Success = true,
                Message = "Lấy báo cáo tồn kho thành công.",
                Data = report,
            });
        }

        private void ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                throw new ApiException("Please select the report type and date range.", 400);
            }

            if (startDate.Value.Date > endDate.Value.Date)
            {
                throw new ApiException("The Start Date cannot be later than the End Date.", 400);
            }
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
