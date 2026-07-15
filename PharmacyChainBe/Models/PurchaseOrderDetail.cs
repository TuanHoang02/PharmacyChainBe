using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class PurchaseOrderDetail
    {
        [Key]
        public int PurchaseOrderDetailID { get; set; }

        public int PurchaseOrderID { get; set; }

        public int MedicineID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public PurchaseOrder? PurchaseOrder { get; set; }
        public Medicine? Medicine { get; set; }
    }
}
