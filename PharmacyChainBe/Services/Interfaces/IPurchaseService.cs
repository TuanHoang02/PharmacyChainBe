using PharmacyChainBe.DTOs.Request;

namespace PharmacyChainBe.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task CreatePurchaseRequestAsync(int branchId, int userId, CreatePurchaseRequestDto request);
        Task ReceiveMedicinesAsync(int branchId, int purchaseRequestId, ReceiveMedicinesDto request);
    }
}
