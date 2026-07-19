using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class CreateStaffDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không quá 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

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

        // Nullable vì nếu BranchManager tạo, hệ thống tự động gán BranchID của chính họ
        public int? BranchID { get; set; }
    }
}
