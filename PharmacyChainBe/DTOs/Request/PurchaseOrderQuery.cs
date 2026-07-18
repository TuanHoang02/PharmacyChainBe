using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Request
{
    public class PurchaseOrderQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public DeliveryStatus? DeliveryStatus { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = true;
    }
}
