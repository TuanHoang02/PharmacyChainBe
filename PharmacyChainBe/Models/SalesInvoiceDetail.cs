using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class SalesInvoiceDetail
    {
        [Key]
        public int SalesInvoiceDetailID { get; set; }

        public int SalesInvoiceID { get; set; }

        public int MedicineID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        // Navigation properties
        [ForeignKey("SalesInvoiceID")]
        public SalesInvoice? SalesInvoice { get; set; }

        [ForeignKey("MedicineID")]
        public Medicine? Medicine { get; set; }
    }
}
