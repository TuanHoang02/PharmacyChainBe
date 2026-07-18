namespace PharmacyChainBe.DTOs.Response
{
    public class UserResponse
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? BranchName { get; set; }
        public bool IsActive { get; set; }
    }
}
