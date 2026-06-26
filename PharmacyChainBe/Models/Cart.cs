using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        public int CustomerID { get; set; }

        public DateTime CreatedDate { get; set; }

        public User? Customer { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
