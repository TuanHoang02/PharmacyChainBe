using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync(
            string? searchTerm, 
            bool? isActive, 
            int pageNumber, 
            int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (items, totalCount) = await _categoryRepository.GetCategoriesAsync(searchTerm, isActive, pageNumber, pageSize);

            var dtos = items.Select(c => MapToDto(c));

            return new PagedResponse<IEnumerable<CategoryDto>>
            {
                Data = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount
            };
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new ApiException("Danh mục không tồn tại.", 404);
            }

            return MapToDto(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request)
        {
            var isDuplicate = await _categoryRepository.ExistsByNameAsync(request.CategoryName);
            if (isDuplicate)
            {
                throw new ApiException("Tên danh mục đã tồn tại.", 400);
            }

            var category = new Category
            {
                CategoryName = request.CategoryName.Trim(),
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdCategory = await _categoryRepository.AddCategoryAsync(category);
            return MapToDto(createdCategory);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto request)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new ApiException("Danh mục không tồn tại.", 404);
            }

            var isDuplicate = await _categoryRepository.ExistsByNameAsync(request.CategoryName, id);
            if (isDuplicate)
            {
                throw new ApiException("Tên danh mục đã tồn tại.", 400);
            }

            category.CategoryName = request.CategoryName.Trim();
            category.Description = request.Description?.Trim();
            category.IsActive = request.IsActive;

            await _categoryRepository.UpdateCategoryAsync(category);
            return MapToDto(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new ApiException("Danh mục không tồn tại.", 404);
            }

            // Soft delete
            category.IsActive = false;
            return await _categoryRepository.UpdateCategoryAsync(category);
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
