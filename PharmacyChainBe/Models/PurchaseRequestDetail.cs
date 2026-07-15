using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class PurchaseRequestDetail
    {
        [Key]
        public int PurchaseRequestDetailID { get; set; }

        public int PurchaseRequestID { get; set; }

        public int MedicineID { get; set; }

        public int Quantity { get; set; }

        public PurchaseRequest? PurchaseRequest { get; set; }
        public Medicine? Medicine { get; set; }
    }
}
