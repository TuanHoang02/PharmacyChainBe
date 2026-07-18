using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IPurchaseRequestService
    {
        Task<PagedResponse<IEnumerable<PurchaseRequestDto>>> GetPurchaseRequestsAsync(PurchaseRequestFilter filter);
        Task<PurchaseRequestDto?> GetPurchaseRequestByIdAsync(int id);
        Task ReviewPurchaseRequestAsync(int id, ReviewPurchaseRequestDto dto, int reviewerUserId);
        Task<IEnumerable<LookupDto>> GetBranchesAsync();
        Task<IEnumerable<LookupDto>> GetSuppliersAsync();
    }
}
