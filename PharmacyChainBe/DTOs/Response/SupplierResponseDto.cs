namespace PharmacyChainBe.DTOs.Response
{
    public class SupplierResponseDto
    {
        public int PurchaseOrderID { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime ConfirmedAt { get; set; }
        public string? RejectionReason { get; set; }
    }
}
