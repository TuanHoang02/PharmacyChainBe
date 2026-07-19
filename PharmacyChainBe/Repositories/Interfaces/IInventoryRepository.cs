using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<(IEnumerable<Inventory> Data, int TotalRecords)> GetInventoriesAsync(int branchId, InventoryRequestDto request);
        Task<Inventory?> GetInventoryAsync(int branchId, int medicineId);
        Task AddStockAsync(int branchId, int medicineId, int quantity);
        Task DeductStockAsync(int branchId, int medicineId, int quantity);
    }
}
