using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class CreatePurchaseRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "A purchase request must contain at least one medicine item.")]
        public List<PurchaseRequestDetailDto> Details { get; set; } = new List<PurchaseRequestDetailDto>();

        [MaxLength(255)]
        public string? Reason { get; set; }
    }

    public class PurchaseRequestDetailDto
    {
        [Required]
        public int MedicineId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Requested quantity must be greater than zero.")]
        public int RequestedQuantity { get; set; }
    }
}
