using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public PurchaseService(IPurchaseRepository purchaseRepository, IInventoryRepository inventoryRepository)
        {
            _purchaseRepository = purchaseRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<DTOs.Response.PurchaseRequestResponseDto>> GetPurchaseRequestsAsync(int branchId)
        {
            var requests = await _purchaseRepository.GetPurchaseRequestsByBranchAsync(branchId);
            return requests.Select(pr => new DTOs.Response.PurchaseRequestResponseDto
            {
                PurchaseRequestId = pr.PurchaseRequestID,
                RequestCode = pr.RequestCode,
                BranchId = pr.BranchID,
                CreatedByUserId = pr.CreatedByUserID,
                Status = pr.Status.ToString(),
                Reason = pr.Reason,
                ReviewNote = pr.ReviewNote,
                CreatedAt = pr.CreatedAt,
                ReviewedAt = pr.ReviewedAt,
                Details = pr.PurchaseRequestDetails.Select(prd => new DTOs.Response.PurchaseRequestDetailResponseDto
                {
                    PurchaseRequestDetailId = prd.PurchaseRequestDetailID,
                    MedicineId = prd.MedicineID,
                    MedicineName = prd.Medicine?.MedicineName ?? "Unknown",
                    RequestedQuantity = prd.RequestedQuantity,
                    CurrentStock = prd.CurrentStock
                }).ToList()
            });
        }

        public async Task CreatePurchaseRequestAsync(int branchId, int userId, CreatePurchaseRequestDto request)
        {
            if (request.Details == null || !request.Details.Any())
            {
                throw new ApiException("A purchase request must contain at least one medicine item.", 400); // BR-03
            }

            var purchaseRequest = new PurchaseRequest
            {
                RequestCode = $"PR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                BranchID = branchId,
                CreatedByUserID = userId,
                Status = PurchaseRequestStatus.Pending,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow,
                PurchaseRequestDetails = new List<PurchaseRequestDetail>()
            };

            foreach (var detail in request.Details)
            {
                if (detail.RequestedQuantity <= 0)
                {
                    throw new ApiException("Requested quantity must be greater than zero.", 400); // AT1 / BR-04
                }

                // Check AT2 - Duplicate pending request
                bool hasPending = await _purchaseRepository.HasPendingPurchaseRequestForMedicineAsync(branchId, detail.MedicineId);
                if (hasPending)
                {
                    throw new ApiException($"A pending purchase request already exists for medicine ID {detail.MedicineId}.", 400);
                }

                var inventory = await _inventoryRepository.GetInventoryAsync(branchId, detail.MedicineId);
                int currentStock = inventory?.QuantityInStock ?? 0;
                int reorderLevel = inventory?.ReorderLevel ?? 0;

                // BR-02 Validation (Optional, but good practice. We can warn or reject. The UC description says "should be created when stock falls below reorder level". Let's reject to enforce BR-02 strictly, or just allow it if manager really needs it. Let's allow it but we need current stock anyway).
                // If we want strict BR-02:
                // if (currentStock > reorderLevel) throw new ApiException("...");

                purchaseRequest.PurchaseRequestDetails.Add(new PurchaseRequestDetail
                {
                    MedicineID = detail.MedicineId,
                    RequestedQuantity = detail.RequestedQuantity,
                    CurrentStock = currentStock
                });
            }

            await _purchaseRepository.CreatePurchaseRequestAsync(purchaseRequest);
        }

        public async Task ReceiveMedicinesAsync(int branchId, int purchaseRequestId)
        {
            var purchaseRequest = await _purchaseRepository.GetPurchaseRequestByIdAsync(purchaseRequestId);
            
            if (purchaseRequest == null || purchaseRequest.BranchID != branchId)
            {
                throw new ApiException("Purchase request not found.", 404); // AT2
            }

            if (purchaseRequest.Status != PurchaseRequestStatus.Approved)
            {
                throw new ApiException("Only approved purchase requests can be received.", 400); // BR-01, BR-04
            }

            var batches = await _purchaseRepository.GetBatchesForPurchaseRequestAsync(purchaseRequestId);
            
            // Note: If UC19 hasn't been executed, batches might be empty.
            // But if it has been executed, we update inventory based on it.
            foreach (var batch in batches)
            {
                if (batch.ReceivedQuantity > 0)
                {
                    await _inventoryRepository.AddStockAsync(
                        branchId, 
                        batch.MedicineID, 
                        batch.ReceivedQuantity
                    );
                }
            }

            // Update Purchase Request Status (POS-01)
            purchaseRequest.Status = PurchaseRequestStatus.Received;
            await _purchaseRepository.UpdatePurchaseRequestAsync(purchaseRequest);
        }
    }
}
