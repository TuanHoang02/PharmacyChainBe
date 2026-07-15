using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class PurchaseRequest
    {
        [Key]
        public int PurchaseRequestID { get; set; }

        public int BranchID { get; set; }

        public int RequestedBy { get; set; }

        public DateTime RequestDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public Branch? Branch { get; set; }
        public User? RequestedUser { get; set; }
        public User? ApprovedUser { get; set; }
        public ICollection<PurchaseRequestDetail> PurchaseRequestDetails { get; set; } = new List<PurchaseRequestDetail>();
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
