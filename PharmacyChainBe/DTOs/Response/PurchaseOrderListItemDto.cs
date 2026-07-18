namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseOrderListItemDto
    {
        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string DeliveryStatus { get; set; } = string.Empty;
    }
}
