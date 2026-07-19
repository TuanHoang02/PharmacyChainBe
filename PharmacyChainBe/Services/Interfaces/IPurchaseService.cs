using PharmacyChainBe.DTOs.Request;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<IEnumerable<DTOs.Response.PurchaseRequestResponseDto>> GetPurchaseRequestsAsync(int branchId);
        Task CreatePurchaseRequestAsync(int branchId, int userId, CreatePurchaseRequestDto request);
        Task ReceiveMedicinesAsync(int branchId, int purchaseRequestId);
    }
}
