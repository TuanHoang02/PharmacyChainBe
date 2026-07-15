using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class MedicineBatch
    {
        [Key]
        public int BatchID { get; set; }

        public int PurchaseOrderID { get; set; }

        public int MedicineID { get; set; }

        [Required]
        public string BatchNumber { get; set; } = string.Empty;

        public DateTime ManufactureDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int Quantity { get; set; }

        public DateTime ReceivedDate { get; set; }

        public PurchaseOrder? PurchaseOrder { get; set; }
        public Medicine? Medicine { get; set; }
    }
}
