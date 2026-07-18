using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseOrderDetailDto
    {
        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string? SupplierResponseNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
    }
}
