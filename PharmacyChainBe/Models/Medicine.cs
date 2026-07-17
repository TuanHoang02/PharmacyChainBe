using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineID { get; set; }

        [Required]
        [MaxLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? GenericName { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        [MaxLength(255)]
        public string? DosageInstructions { get; set; }

        public bool RequiresPrescription { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();
        public ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();
        public ICollection<PurchaseRequestDetail> PurchaseRequestDetails { get; set; } = new List<PurchaseRequestDetail>();
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
