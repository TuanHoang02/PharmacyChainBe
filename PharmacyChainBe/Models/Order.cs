using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public int PharmacistID { get; set; }

        public int BranchID { get; set; }

        public int? PrescriptionID { get; set; }

        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public User? Pharmacist { get; set; }

        public Branch? Branch { get; set; }

        public Prescription? Prescription { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
