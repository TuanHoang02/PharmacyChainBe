using PharmacyChainBe.Models;

namespace PharmacyChainBe.Repositories.Interfaces
{
    public interface ISalesRepository
    {
        Task<SalesInvoice> CreateSalesInvoiceAsync(SalesInvoice invoice);
        Task<Medicine?> GetMedicineByIdAsync(int medicineId);
    }
}
