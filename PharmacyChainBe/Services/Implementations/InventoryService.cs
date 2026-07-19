using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<PagedResponse<IEnumerable<InventoryResponseDto>>> GetInventoriesAsync(int branchId, InventoryRequestDto request)
        {
            var (data, totalRecords) = await _inventoryRepository.GetInventoriesAsync(branchId, request);

            var responseData = data.Select(i => new InventoryResponseDto
            {
                InventoryId = i.InventoryID,
                BranchId = i.BranchID,
                MedicineId = i.MedicineID,
                MedicineName = i.Medicine != null ? i.Medicine.MedicineName : "Unknown",
                CategoryName = i.Medicine?.Category != null ? i.Medicine.Category.CategoryName : "Unknown",
                QuantityInStock = i.QuantityInStock,
                ReorderLevel = i.ReorderLevel,
                IsLowStock = i.QuantityInStock <= i.ReorderLevel,
                SellingPrice = i.Medicine?.SellingPrice ?? 0,
                RequiresPrescription = i.Medicine?.RequiresPrescription ?? false,
                LastUpdatedAt = i.LastUpdatedAt
            });

            return new PagedResponse<IEnumerable<InventoryResponseDto>>
            {
                Data = responseData,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }

    }
}
