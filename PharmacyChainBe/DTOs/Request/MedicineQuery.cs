namespace PharmacyChainBe.DTOs.Request
{
    public class MedicineQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}
