using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public int RoleID { get; set; }

        public int? BranchID { get; set; }

        public int? SupplierID { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(1000)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("RoleID")]
        public Role? Role { get; set; }

        [ForeignKey("BranchID")]
        public Branch? Branch { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier? Supplier { get; set; }

        public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
        public ICollection<PurchaseRequest> CreatedPurchaseRequests { get; set; } = new List<PurchaseRequest>();
        public ICollection<PurchaseRequest> ReviewedPurchaseRequests { get; set; } = new List<PurchaseRequest>();
        public ICollection<PurchaseOrder> CreatedPurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public ICollection<PurchaseOrder> ReceivedPurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
