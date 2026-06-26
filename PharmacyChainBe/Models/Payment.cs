using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        public string TransactionCode { get; set; } = string.Empty;

        public int OrderID { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public Order? Order { get; set; }
    }
}
