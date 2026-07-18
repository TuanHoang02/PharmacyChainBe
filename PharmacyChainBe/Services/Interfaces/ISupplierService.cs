using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<PagedResponse<IEnumerable<SupplierDto>>> GetSuppliersAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
        Task<SupplierDto> GetSupplierByIdAsync(int id);
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto request);
        Task<SupplierDto> UpdateSupplierAsync(int id, UpdateSupplierDto request);
        Task<bool> DeleteSupplierAsync(int id);
    }
}
