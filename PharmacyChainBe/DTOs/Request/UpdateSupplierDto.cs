using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class UpdateSupplierDto
    {
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [StringLength(100, ErrorMessage = "Tên nhà cung cấp không quá 100 ký tự")]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Tên người liên hệ không quá 100 ký tự")]
        public string? ContactName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không quá 20 ký tự")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không quá 100 ký tự")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không quá 255 ký tự")]
        public string? Address { get; set; }

        public bool IsActive { get; set; }
    }
}
