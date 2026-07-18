using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseOrderFilterRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? BranchId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
