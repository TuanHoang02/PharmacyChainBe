using PharmacyChainBe.DTOs;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<PagedResponse<List<PurchaseOrder>>> GetBySupplierPagedAsync(
            int supplierId,
            int pageNumber,
            int pageSize,
            string? search,
            int? branchId,
            DateTime? startDate,
            DateTime? endDate,
            OrderStatus? status);

        Task<PurchaseOrder?> GetByIdAndSupplierAsync(int purchaseOrderId, int supplierId);

        // ponytail: returns a tracked entity so the caller can mutate fields and persist
        // via UpdateAsync; ceiling is full-row load, upgrade to ExecuteUpdateAsync if the
        // update set ever grows beyond 3 columns.
        Task<PurchaseOrder?> GetByIdAndSupplierForUpdateAsync(int purchaseOrderId, int supplierId);

        Task UpdateAsync(PurchaseOrder purchaseOrder);
    }
}
