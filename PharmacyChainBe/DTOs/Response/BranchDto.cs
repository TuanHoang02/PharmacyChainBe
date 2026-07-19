namespace PharmacyChainBe.DTOs.Response
{
    public class BranchDto
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
