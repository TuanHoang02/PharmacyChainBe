using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface ISalesRepository
    {
        Task<PagedResponse<List<SalesInvoice>>> GetPagedAsync(SalesHistoryQuery query, CancellationToken cancellationToken = default);
        Task<SalesInvoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SalesInvoice> CreateSalesInvoiceAsync(SalesInvoice invoice);
        Task<Medicine?> GetMedicineByIdAsync(int medicineId);
    }
}
