using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("BranchID")]
        public Branch? Branch { get; set; }

        [ForeignKey("MedicineID")]
        public Medicine? Medicine { get; set; }
    }
}
