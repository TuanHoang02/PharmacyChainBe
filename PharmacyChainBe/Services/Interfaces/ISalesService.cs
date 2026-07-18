using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface ISalesService
    {
        Task<PagedResponse<List<SalesHistoryDto>>> GetPagedAsync(SalesHistoryQuery query, CancellationToken cancellationToken = default);
        Task<SalesInvoiceDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
