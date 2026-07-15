using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderID { get; set; }

        public int PurchaseRequestID { get; set; }

        public int SupplierID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public PurchaseRequest? PurchaseRequest { get; set; }
        public Supplier? Supplier { get; set; }
        public User? CreatedUser { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
        public ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();
    }
}
