using PharmacyChainBe.Enums;
using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class CreateSalesInvoiceDto
    {
        [MaxLength(100)]
        public string? CustomerName { get; set; }

        [MaxLength(20)]
        public string? CustomerPhoneNumber { get; set; }

        public decimal DiscountAmount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public string? PrescriptionImageUrl { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A sales invoice must contain at least one medicine item.")]
        public List<SalesInvoiceDetailDto> Details { get; set; } = new List<SalesInvoiceDetailDto>();
    }

    public class SalesInvoiceDetailDto
    {
        [Required]
        public int MedicineId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}
