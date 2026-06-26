using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }

        public int BranchID { get; set; }

        public int MedicineID { get; set; }

        public int QuantityInStock { get; set; }

        public int ReorderLevel { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? UpdatedBy { get; set; }

        public User? UpdatedUser { get; set; }

        public Branch? Branch { get; set; }

        public Medicine? Medicine { get; set; }
    }
}
