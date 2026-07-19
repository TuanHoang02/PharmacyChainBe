using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface ISalesService
    {
        Task<PagedResponse<List<SalesHistoryDto>>> GetPagedAsync(SalesHistoryQuery query, CancellationToken cancellationToken = default);
        Task<DTOs.Response.SalesInvoiceDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task CreateSalesInvoiceAsync(int branchId, int userId, CreateSalesInvoiceDto request);
    }
}
