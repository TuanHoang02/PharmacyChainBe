namespace PharmacyChainBe.DTOs.Request
{
    public class InventoryRequestDto
    {
        public int? CategoryId { get; set; }
        public string? SearchKeyword { get; set; }
        public bool? IsLowStock { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
