using PharmacyChainBe.DTOs.Request;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface ISalesService
    {
        Task CreateSalesInvoiceAsync(int branchId, int userId, CreateSalesInvoiceDto request);
    }
}
