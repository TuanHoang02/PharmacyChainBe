using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IMedicineRepository
    {
        Task<Medicine?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Medicine?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<PagedResponse<List<Medicine>>> GetPagedAsync(MedicineQuery query, CancellationToken cancellationToken = default);
        Task<Medicine> AddAsync(Medicine medicine, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Medicine medicine, CancellationToken cancellationToken = default);
    }
}
