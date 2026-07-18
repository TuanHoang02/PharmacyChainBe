using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<(IEnumerable<Category> Items, int TotalCount)> GetCategoriesAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> AddCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
