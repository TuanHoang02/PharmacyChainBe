using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class UpdateBranchDto
    {
        [Required(ErrorMessage = "Tên chi nhánh không được để trống")]
        [StringLength(100, ErrorMessage = "Tên chi nhánh không quá 100 ký tự")]
        public string BranchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(255, ErrorMessage = "Địa chỉ không quá 255 ký tự")]
        public string Address { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không quá 20 ký tự")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không quá 100 ký tự")]
        public string? Email { get; set; }

        public bool IsActive { get; set; }
    }
}
