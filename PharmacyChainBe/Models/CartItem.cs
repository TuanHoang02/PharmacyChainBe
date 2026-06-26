using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        public int CartID { get; set; }

        public int MedicineID { get; set; }

        public int Quantity { get; set; }

        public Cart? Cart { get; set; }

        public Medicine? Medicine { get; set; }
    }
}
