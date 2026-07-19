using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<PagedResponse<IEnumerable<InventoryResponseDto>>> GetInventoriesAsync(int branchId, InventoryRequestDto request);
    }
}
