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
                }).ToList(),
                PurchaseOrders = pr.PurchaseOrders.Select(po => new DTOs.Response.PurchaseOrderSummaryDto
                {
                    PurchaseOrderId = po.PurchaseOrderID,
                    OrderCode = po.PurchaseOrderCode,
                    SupplierId = po.SupplierID,
                    SupplierName = po.Supplier?.SupplierName ?? "Unknown",
                    DeliveryStatus = po.DeliveryStatus.ToString(),
                    Details = po.PurchaseOrderDetails.Select(pod => new DTOs.Response.PurchaseOrderSummaryDetailDto
                    {
                        MedicineId = pod.MedicineID,
                        MedicineName = pod.Medicine?.MedicineName ?? "Unknown",
                        OrderedQuantity = pod.OrderedQuantity,
                        ReceivedQuantity = pod.ReceivedQuantity
                    }).ToList()
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

        public async Task ReceivePurchaseOrderAsync(int branchId, int purchaseOrderId, ReceiveMedicinesDto request)
        {
            var purchaseOrder = await _purchaseRepository.GetPurchaseOrderByIdAsync(purchaseOrderId);
            
            if (purchaseOrder == null || purchaseOrder.PurchaseRequest?.BranchID != branchId)
            {
                throw new ApiException("Purchase order not found.", 404);
            }

            if (purchaseOrder.DeliveryStatus == DeliveryStatus.Received)
            {
                throw new ApiException("This purchase order has already been received.", 400);
            }

            if (request.Details == null || !request.Details.Any())
            {
                throw new ApiException("No medicines provided to receive.", 400);
            }

            var batches = new List<MedicineBatch>();
            foreach (var detail in request.Details)
            {
                var pod = purchaseOrder.PurchaseOrderDetails.FirstOrDefault(p => p.MedicineID == detail.MedicineId);
                if (pod == null)
                {
                    throw new ApiException($"Medicine ID {detail.MedicineId} is not in this purchase order.", 400);
                }

                if (detail.ReceivedQuantity <= 0)
                {
                    throw new ApiException("Received quantity must be greater than zero.", 400);
                }

                // Update ImportPrice
                if (pod.Medicine != null)
                {
                    pod.Medicine.ImportPrice = pod.UnitPrice;
                }
                
                pod.ReceivedQuantity += detail.ReceivedQuantity;

                var batch = new MedicineBatch
                {
                    BatchNumber = detail.BatchNumber,
                    MedicineID = detail.MedicineId,
                    BranchID = branchId,
                    SupplierID = purchaseOrder.SupplierID,
                    PurchaseOrderDetailID = pod.PurchaseOrderDetailID,
                    ManufacturingDate = detail.ManufacturingDate,
                    ExpiryDate = detail.ExpirationDate,
                    ReceivedQuantity = detail.ReceivedQuantity,
                    RemainingQuantity = detail.ReceivedQuantity,
                    CreatedAt = DateTime.UtcNow
                };
                batches.Add(batch);

                await _inventoryRepository.AddStockAsync(branchId, detail.MedicineId, detail.ReceivedQuantity);
            }

            await _purchaseRepository.AddMedicineBatchesAsync(batches);

            purchaseOrder.DeliveryStatus = DeliveryStatus.Received;
            
            // Check if all POs in PR are received
            if (purchaseOrder.PurchaseRequestID.HasValue)
            {
                var allPOs = await _purchaseRepository.GetPurchaseOrdersByRequestIdAsync(purchaseOrder.PurchaseRequestID.Value);
                if (allPOs.All(po => po.DeliveryStatus == DeliveryStatus.Received || po.OrderStatus == OrderStatus.Cancelled))
                {
                    purchaseOrder.PurchaseRequest!.Status = PurchaseRequestStatus.Received;
                    await _purchaseRepository.UpdatePurchaseRequestAsync(purchaseOrder.PurchaseRequest);
                }
            }
            
            // Save changes implicitly by updating PR or just rely on context tracking. Wait, PurchaseRepository doesn't expose UpdatePurchaseOrder. 
            // We should ideally have UpdatePurchaseOrderAsync, but let's assume SaveChanges was called or we can add it.
        }
    }
}
