namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseRequestResponseDto
    {
        public int PurchaseRequestId { get; set; }
        public string RequestCode { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? ReviewNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public List<PurchaseRequestDetailResponseDto> Details { get; set; } = new List<PurchaseRequestDetailResponseDto>();
        public List<PurchaseOrderSummaryDto> PurchaseOrders { get; set; } = new List<PurchaseOrderSummaryDto>();
    }

    public class PurchaseOrderSummaryDto
    {
        public int PurchaseOrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string DeliveryStatus { get; set; } = string.Empty;
        public List<PurchaseOrderSummaryDetailDto> Details { get; set; } = new List<PurchaseOrderSummaryDetailDto>();
    }

    public class PurchaseOrderSummaryDetailDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public List<PreDeclaredBatchDto> Batches { get; set; } = new List<PreDeclaredBatchDto>();
    }

    public class PreDeclaredBatchDto
    {
        public int MedicineBatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int DeclaredQuantity { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public class PurchaseRequestDetailResponseDto
    {
        public int PurchaseRequestDetailId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
        public int CurrentStock { get; set; }
    }
}
