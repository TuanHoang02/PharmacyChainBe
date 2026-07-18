using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Response
{
    public class SalesHistoryDto
    {
        public int SalesInvoiceID { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
