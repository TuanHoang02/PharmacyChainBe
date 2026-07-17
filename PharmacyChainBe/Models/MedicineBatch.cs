using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class MedicineBatch
    {
        [Key]
        public int MedicineBatchID { get; set; }

        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        public int MedicineID { get; set; }

        public int BranchID { get; set; }

        public int SupplierID { get; set; }

        public int? PurchaseOrderDetailID { get; set; }

        [Column(TypeName = "date")]
        public DateTime ManufacturingDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

        public int ReceivedQuantity { get; set; }

        public int RemainingQuantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("MedicineID")]
        public Medicine? Medicine { get; set; }

        [ForeignKey("BranchID")]
        public Branch? Branch { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier? Supplier { get; set; }

        [ForeignKey("PurchaseOrderDetailID")]
        public PurchaseOrderDetail? PurchaseOrderDetail { get; set; }
    }
}
