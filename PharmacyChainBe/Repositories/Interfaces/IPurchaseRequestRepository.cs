using PharmacyChainBe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using PharmacyChainBe.DTOs.Request;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IPurchaseRequestRepository
    {
        Task<(IEnumerable<PurchaseRequest> Requests, int TotalRecords)> GetPurchaseRequestsAsync(PurchaseRequestFilter filter);
        Task<PurchaseRequest?> GetPurchaseRequestByIdAsync(int id);
        Task UpdatePurchaseRequestAsync(PurchaseRequest request);
        Task<IEnumerable<Branch>> GetBranchesAsync();
        Task<IEnumerable<Supplier>> GetSuppliersAsync();
        Task CreatePurchaseOrderAsync(PurchaseOrder order);
    }
}
