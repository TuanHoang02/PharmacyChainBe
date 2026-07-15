using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [ForeignKey(nameof(Role))]
        public int RoleID { get; set; }

        [ForeignKey(nameof(Branch))]
        public int? BranchID { get; set; }

        [ForeignKey(nameof(Supplier))]
        public int? SupplierID { get; set; }

        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Role? Role { get; set; }

        public Branch? Branch { get; set; }

        public Supplier? Supplier { get; set; }
    }
}
