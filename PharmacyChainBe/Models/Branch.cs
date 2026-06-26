using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class Branch
    {
        [Key]
        public int BranchID { get; set; }

        [Required]
        public string BranchName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public User? CreatedUser { get; set; }

        public User? UpdatedUser { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
