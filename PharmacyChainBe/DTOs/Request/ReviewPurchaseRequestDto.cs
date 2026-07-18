using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class ReviewPurchaseRequestDto
    {
        [Required]
        public bool IsApproved { get; set; }

        public string? RejectionReason { get; set; }

        public List<DetailSupplierDto>? DetailSuppliers { get; set; }
    }

    public class DetailSupplierDto
    {
        public int PurchaseRequestDetailID { get; set; }
        public int SupplierID { get; set; }
    }
}
