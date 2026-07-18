using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmacyChainBe.Services.Implementations
{
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly IPurchaseRequestRepository _repository;

        public PurchaseRequestService(IPurchaseRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<IEnumerable<PurchaseRequestDto>>> GetPurchaseRequestsAsync(PurchaseRequestFilter filter)
        {
            var result = await _repository.GetPurchaseRequestsAsync(filter);
            
            var dtos = result.Requests.Select(pr => new PurchaseRequestDto
            {
                PurchaseRequestID = pr.PurchaseRequestID,
                RequestCode = pr.RequestCode,
                BranchID = pr.BranchID,
                BranchName = pr.Branch?.BranchName,
                CreatedByUserID = pr.CreatedByUserID,
                CreatedByUserName = pr.CreatedByUser?.FullName,
                Status = pr.Status.ToString(),
                Reason = pr.Reason,
                ReviewNote = pr.ReviewNote,
                ReviewedByUserID = pr.ReviewedByUserID,
                ReviewedByUserName = pr.ReviewedByUser?.FullName,
                CreatedAt = pr.CreatedAt,
                ReviewedAt = pr.ReviewedAt
            }).ToList();

            return new PagedResponse<IEnumerable<PurchaseRequestDto>>
            {
                Data = dtos,
                PageNumber = filter.Page,
                PageSize = filter.Size,
                TotalRecords = result.TotalRecords
            };
        }

        public async Task<PurchaseRequestDto?> GetPurchaseRequestByIdAsync(int id)
        {
            var pr = await _repository.GetPurchaseRequestByIdAsync(id);
            if (pr == null)
            {
                throw new ApiException($"Purchase request with ID {id} not found.", 404);
            }

            return new PurchaseRequestDto
            {
                PurchaseRequestID = pr.PurchaseRequestID,
                RequestCode = pr.RequestCode,
                BranchID = pr.BranchID,
                BranchName = pr.Branch?.BranchName,
                CreatedByUserID = pr.CreatedByUserID,
                CreatedByUserName = pr.CreatedByUser?.FullName,
                Status = pr.Status.ToString(),
                Reason = pr.Reason,
                ReviewNote = pr.ReviewNote,
                ReviewedByUserID = pr.ReviewedByUserID,
                ReviewedByUserName = pr.ReviewedByUser?.FullName,
                CreatedAt = pr.CreatedAt,
                ReviewedAt = pr.ReviewedAt,
                Details = pr.PurchaseRequestDetails.Select(prd => new PurchaseRequestDetailDto
                {
                    PurchaseRequestDetailID = prd.PurchaseRequestDetailID,
                    PurchaseRequestID = prd.PurchaseRequestID,
                    MedicineID = prd.MedicineID,
                    MedicineName = prd.Medicine?.MedicineName,
                    Unit = prd.Medicine?.Unit,
                    CurrentStock = prd.CurrentStock,
                    RequestedQuantity = prd.RequestedQuantity
                }).ToList(),
                PurchaseOrders = pr.PurchaseOrders?.Select(po => new PurchaseRequestOrderDto
                {
                    PurchaseOrderID = po.PurchaseOrderID,
                    PurchaseOrderCode = po.PurchaseOrderCode,
                    SupplierName = po.Supplier?.SupplierName ?? string.Empty,
                    OrderStatus = po.OrderStatus.ToString(),
                    DeliveryStatus = po.DeliveryStatus.ToString(),
                    TotalAmount = po.TotalAmount,
                    CreatedAt = po.CreatedAt,
                    Items = po.PurchaseOrderDetails?.Select(pod => new PurchaseRequestOrderItemDto
                    {
                        MedicineName = pod.Medicine?.MedicineName ?? string.Empty,
                        OrderedQuantity = pod.OrderedQuantity,
                        Unit = pod.Medicine?.Unit ?? string.Empty
                    }).ToList() ?? new List<PurchaseRequestOrderItemDto>()
                }).ToList() ?? new List<PurchaseRequestOrderDto>()
            };
        }

        public async Task ReviewPurchaseRequestAsync(int id, ReviewPurchaseRequestDto dto, int reviewerUserId)
        {
            var pr = await _repository.GetPurchaseRequestByIdAsync(id);
            if (pr == null)
            {
                throw new ApiException($"Purchase request with ID {id} not found.", 404);
            }

            if (pr.Status != PurchaseRequestStatus.Pending)
            {
                throw new ApiException("This purchase request has already been processed.", 400);
            }

            if (!dto.IsApproved && string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                throw new ApiException("Rejection reason is required.", 400);
            }

            if (dto.IsApproved)
            {
                if (dto.DetailSuppliers == null || !dto.DetailSuppliers.Any())
                {
                    throw new ApiException("Supplier mapping is required for each requested medicine.", 400);
                }

                // Check if all details have a supplier
                var requestDetailIds = pr.PurchaseRequestDetails.Select(d => d.PurchaseRequestDetailID).ToList();
                var providedDetailIds = dto.DetailSuppliers.Select(d => d.PurchaseRequestDetailID).ToList();
                var missingDetails = requestDetailIds.Except(providedDetailIds);
                if (missingDetails.Any())
                {
                    throw new ApiException("Please select a supplier for all requested medicines.", 400);
                }
            }

            pr.Status = dto.IsApproved ? PurchaseRequestStatus.Approved : PurchaseRequestStatus.Rejected;
            pr.ReviewNote = dto.IsApproved ? null : dto.RejectionReason;
            pr.ReviewedByUserID = reviewerUserId;
            pr.ReviewedAt = DateTime.UtcNow;

            await _repository.UpdatePurchaseRequestAsync(pr);

            // Automatically create Purchase Orders grouped by Supplier
            if (dto.IsApproved && dto.DetailSuppliers != null)
            {
                var groupedBySupplier = dto.DetailSuppliers.GroupBy(ds => ds.SupplierID);

                foreach (var group in groupedBySupplier)
                {
                    var supplierId = group.Key;
                    var detailIdsForSupplier = group.Select(ds => ds.PurchaseRequestDetailID).ToList();
                    var matchedDetails = pr.PurchaseRequestDetails.Where(d => detailIdsForSupplier.Contains(d.PurchaseRequestDetailID)).ToList();

                    var po = new Models.PurchaseOrder
                    {
                        PurchaseOrderCode = "PO-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper(),
                        BranchID = pr.BranchID,
                        SupplierID = supplierId,
                        PurchaseRequestID = pr.PurchaseRequestID,
                        CreatedByUserID = reviewerUserId,
                        OrderStatus = OrderStatus.PendingSupplierConfirmation,
                        DeliveryStatus = DeliveryStatus.NotStarted,
                        CreatedAt = DateTime.UtcNow,
                        TotalAmount = 0
                    };

                    foreach (var detail in matchedDetails)
                    {
                        po.PurchaseOrderDetails.Add(new Models.PurchaseOrderDetail
                        {
                            MedicineID = detail.MedicineID,
                            OrderedQuantity = detail.RequestedQuantity,
                            ReceivedQuantity = 0,
                            UnitPrice = 0,
                            LineTotal = 0
                        });
                    }

                    await _repository.CreatePurchaseOrderAsync(po);
                }
            }
        }

        public async Task<IEnumerable<LookupDto>> GetBranchesAsync()
        {
            var branches = await _repository.GetBranchesAsync();
            return branches.Select(b => new LookupDto
            {
                Id = b.BranchID,
                Name = b.BranchName
            });
        }

        public async Task<IEnumerable<LookupDto>> GetSuppliersAsync()
        {
            var suppliers = await _repository.GetSuppliersAsync();
            return suppliers.Select(s => new LookupDto
            {
                Id = s.SupplierID,
                Name = s.SupplierName
            });
        }
    }
}
