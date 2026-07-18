namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseOrderDetailDto
    {
        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
        public string? BranchPhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string DeliveryStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? CreatedByFullName { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
    }
}
