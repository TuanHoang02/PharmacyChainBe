using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class UpdateMedicineRequest
    {
        [Required(ErrorMessage = "Tên thuốc không được để trống.")]
        [MaxLength(200, ErrorMessage = "Tên thuốc không được vượt quá 200 ký tự.")]
        public string MedicineName { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Tên gốc không được vượt quá 200 ký tự.")]
        public string? GenericName { get; set; }

        [Required(ErrorMessage = "Mã danh mục không được để trống.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống.")]
        [Range(0.01, 10000000.00, ErrorMessage = "Giá bán phải lớn hơn 0 và nhỏ hơn hoặc bằng 10.000.000.")]
        public decimal SellingPrice { get; set; }

        [Required(ErrorMessage = "Đơn vị tính không được để trống.")]
        public string Unit { get; set; } = string.Empty;

        public string? DosageInstructions { get; set; }

        public bool RequiresPrescription { get; set; }
    }
}
