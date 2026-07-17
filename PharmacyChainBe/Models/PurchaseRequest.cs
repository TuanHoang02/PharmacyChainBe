using PharmacyChainBe.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class PurchaseRequest
    {
        [Key]
        public int PurchaseRequestID { get; set; }

        [Required]
        [MaxLength(50)]
        public string RequestCode { get; set; } = string.Empty;

        public int BranchID { get; set; }

        public int CreatedByUserID { get; set; }

        public PurchaseRequestStatus Status { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }

        [MaxLength(255)]
        public string? ReviewNote { get; set; }

        public int? ReviewedByUserID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        // Navigation properties
        [ForeignKey("BranchID")]
        public Branch? Branch { get; set; }

        [ForeignKey("CreatedByUserID")]
        public User? CreatedByUser { get; set; }

        [ForeignKey("ReviewedByUserID")]
        public User? ReviewedByUser { get; set; }

        public ICollection<PurchaseRequestDetail> PurchaseRequestDetails { get; set; } = new List<PurchaseRequestDetail>();
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
