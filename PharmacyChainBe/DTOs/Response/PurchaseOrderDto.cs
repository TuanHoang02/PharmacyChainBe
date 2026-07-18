using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseOrderDto
    {
        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
