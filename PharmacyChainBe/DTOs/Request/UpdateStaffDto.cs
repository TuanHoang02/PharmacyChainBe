using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class UpdateStaffDto
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không quá 100 ký tự")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không quá 20 ký tự")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Quyền (RoleID) là bắt buộc")]
        public int RoleID { get; set; }

        public int? BranchID { get; set; }

        public bool IsActive { get; set; }
    }
}
