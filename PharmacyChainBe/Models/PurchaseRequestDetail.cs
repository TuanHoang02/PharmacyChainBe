using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class PurchaseRequestDetail
    {
        [Key]
        public int PurchaseRequestDetailID { get; set; }

        public int PurchaseRequestID { get; set; }

        public int MedicineID { get; set; }

        public int CurrentStock { get; set; }

        public int RequestedQuantity { get; set; }

        // Navigation properties
        [ForeignKey("PurchaseRequestID")]
        public PurchaseRequest? PurchaseRequest { get; set; }

        [ForeignKey("MedicineID")]
        public Medicine? Medicine { get; set; }
    }
}
