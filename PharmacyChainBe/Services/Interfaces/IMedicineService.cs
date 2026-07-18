using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IMedicineService
    {
        Task<PagedResponse<List<MedicineDto>>> GetPagedAsync(MedicineQuery query, CancellationToken cancellationToken = default);
        Task<MedicineDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<MedicineDetailDto> CreateAsync(CreateMedicineRequest request, CancellationToken cancellationToken = default);
        Task<MedicineDetailDto> UpdateAsync(int id, UpdateMedicineRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken = default);
    }
}
