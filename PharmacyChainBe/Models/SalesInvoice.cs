using PharmacyChainBe.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class SalesInvoice
    {
        [Key]
        public int SalesInvoiceID { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceCode { get; set; } = string.Empty;

        public int BranchID { get; set; }

        public int CreatedByUserID { get; set; }

        [MaxLength(100)]
        public string? CustomerName { get; set; }

        [MaxLength(20)]
        public string? CustomerPhoneNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }

        [MaxLength(255)]
        public string? PrescriptionImageUrl { get; set; }

        public bool? IsPrescriptionVerified { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("BranchID")]
        public Branch? Branch { get; set; }

        [ForeignKey("CreatedByUserID")]
        public User? CreatedByUser { get; set; }

        public ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();
    }
}
