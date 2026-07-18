using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Request
{
    public class PurchaseRequestFilter
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public PurchaseRequestStatus? Status { get; set; }
        public int? BranchID { get; set; }
    }
}
