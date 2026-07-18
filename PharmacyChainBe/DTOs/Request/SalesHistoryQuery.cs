using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Request
{
    public class SalesHistoryQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public InvoiceStatus? InvoiceStatus { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = true;
    }
}
