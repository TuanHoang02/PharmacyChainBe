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
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _supplierService.GetSuppliersAsync(searchTerm, isActive, pageNumber, pageSize);
            return Ok(new BaseApiResponse<PagedResponse<IEnumerable<SupplierDto>>>
            {
                Success = true,
                Message = "Lấy danh sách nhà cung cấp thành công.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var result = await _supplierService.GetSupplierByIdAsync(id);
            return Ok(new BaseApiResponse<SupplierDto>
            {
                Success = true,
                Message = "Lấy chi tiết nhà cung cấp thành công.",
                Data = result
            });
        }

        [HttpPost]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto request)
        {
            var result = await _supplierService.CreateSupplierAsync(request);
            return CreatedAtAction(nameof(GetSupplierById), new { id = result.SupplierID }, new BaseApiResponse<SupplierDto>
            {
                Success = true,
                Message = "Tạo nhà cung cấp thành công.",
                Data = result
            });
        }

        [HttpPost("register")]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> CreateSupplierWithUser([FromBody] CreateSupplierWithUserDto request)
        {
            var result = await _supplierService.CreateSupplierWithUserAsync(request);
            return CreatedAtAction(nameof(GetSupplierById), new { id = result.SupplierID }, new BaseApiResponse<SupplierDto>
            {
                Success = true,
                Message = "Tạo nhà cung cấp và tài khoản thành công.",
                Data = result
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierDto request)
        {
            var result = await _supplierService.UpdateSupplierAsync(id, request);
            return Ok(new BaseApiResponse<SupplierDto>
            {
                Success = true,
                Message = "Cập nhật nhà cung cấp thành công.",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (result)
            {
                return Ok(new BaseApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa nhà cung cấp thành công (Xóa mềm)."
                });
            }

            return BadRequest(new BaseApiResponse<object>
            {
                Success = false,
                Message = "Xóa nhà cung cấp thất bại."
            });
        }
    }
}
