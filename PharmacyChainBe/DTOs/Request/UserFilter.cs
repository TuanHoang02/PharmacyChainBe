namespace PharmacyChainBe.DTOs.Request
{
    public class UserFilter
    {
        public string? Keyword { get; set; }
        public int? RoleId { get; set; }
        public int? BranchID { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
