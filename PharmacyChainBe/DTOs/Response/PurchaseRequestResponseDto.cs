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
