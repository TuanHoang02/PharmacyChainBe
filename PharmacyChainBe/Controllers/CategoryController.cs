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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _categoryService.GetCategoriesAsync(searchTerm, isActive, pageNumber, pageSize);
            return Ok(new BaseApiResponse<PagedResponse<IEnumerable<CategoryDto>>>
            {
                Success = true,
                Message = "Lấy danh sách danh mục thành công.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(new BaseApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Lấy chi tiết danh mục thành công.",
                Data = result
            });
        }

        [HttpPost]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.CategoryID }, new BaseApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Tạo danh mục thành công.",
                Data = result
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);
            return Ok(new BaseApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Cập nhật danh mục thành công.",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "OperationsManager,Operations Manager")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result)
            {
                return Ok(new BaseApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa danh mục thành công (Xóa mềm)."
                });
            }

            return BadRequest(new BaseApiResponse<object>
            {
                Success = false,
                Message = "Xóa danh mục thất bại."
            });
        }
    }
}
