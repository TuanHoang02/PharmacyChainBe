using PharmacyChainBe.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderID { get; set; }

        [Required]
        [MaxLength(50)]
        public string PurchaseOrderCode { get; set; } = string.Empty;

        public int BranchID { get; set; }

        public int SupplierID { get; set; }

        public int? PurchaseRequestID { get; set; }

        public int CreatedByUserID { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(255)]
        public string? SupplierResponseNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public int? ReceivedByUserID { get; set; }

        // Navigation properties
        [ForeignKey("BranchID")]
        public Branch? Branch { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier? Supplier { get; set; }

        [ForeignKey("PurchaseRequestID")]
        public PurchaseRequest? PurchaseRequest { get; set; }

        [ForeignKey("CreatedByUserID")]
        public User? CreatedByUser { get; set; }

        [ForeignKey("ReceivedByUserID")]
        public User? ReceivedByUser { get; set; }

        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
