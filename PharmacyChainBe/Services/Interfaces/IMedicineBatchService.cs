using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IMedicineBatchService
    {
        Task<PagedResponse<List<MedicineBatchDto>>> GetPagedAsync(int userId, MedicineBatchQuery query, CancellationToken cancellationToken = default);
        Task<MedicineBatchDetailDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
        Task<MedicineBatchDetailDto> CreateAsync(int userId, CreateMedicineBatchRequest request, CancellationToken cancellationToken = default);
    }
}
