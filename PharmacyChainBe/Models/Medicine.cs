using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyChainBe.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineID { get; set; }

        [Required]
        public string MedicineName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string MedicineCode { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public string Manufacturer { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public bool PrescriptionRequired { get; set; }

        public string? ImageUrl { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public Category? Category { get; set; }

        public User? CreatedUser { get; set; }

        public User? UpdatedUser { get; set; }

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
