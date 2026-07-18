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
    [Authorize]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBranches(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _branchService.GetBranchesAsync(searchTerm, isActive, pageNumber, pageSize);
            return Ok(new BaseApiResponse<PagedResponse<IEnumerable<BranchDto>>>
            {
                Success = true,
                Message = "Lấy danh sách chi nhánh thành công.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var result = await _branchService.GetBranchByIdAsync(id);
            return Ok(new BaseApiResponse<BranchDto>
            {
                Success = true,
                Message = "Lấy chi tiết chi nhánh thành công.",
                Data = result
            });
        }

        [HttpPost]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchDto request)
        {
            var result = await _branchService.CreateBranchAsync(request);
            return CreatedAtAction(nameof(GetBranchById), new { id = result.BranchID }, new BaseApiResponse<BranchDto>
            {
                Success = true,
                Message = "Tạo chi nhánh thành công.",
                Data = result
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] UpdateBranchDto request)
        {
            var result = await _branchService.UpdateBranchAsync(id, request);
            return Ok(new BaseApiResponse<BranchDto>
            {
                Success = true,
                Message = "Cập nhật chi nhánh thành công.",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _branchService.DeleteBranchAsync(id);
            if (result)
            {
                return Ok(new BaseApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa chi nhánh thành công (Xóa mềm)."
                });
            }

            return BadRequest(new BaseApiResponse<object>
            {
                Success = false,
                Message = "Xóa chi nhánh thất bại."
            });
        }
    }
}
