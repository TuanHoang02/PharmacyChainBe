using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<(IEnumerable<Supplier> Items, int TotalCount)> GetSuppliersAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
        Task<Supplier?> GetSupplierByIdAsync(int id);
        Task<Supplier> AddSupplierAsync(Supplier supplier);
        Task<bool> UpdateSupplierAsync(Supplier supplier);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
