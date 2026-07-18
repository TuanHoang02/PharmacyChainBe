using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto request);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
