using PharmacyChainBe.Enums;

namespace PharmacyChainBe.DTOs.Response
{
    public class SalesInvoiceDetailDto
    {
        public int SalesInvoiceID { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public string? PrescriptionImageUrl { get; set; }
        public bool? IsPrescriptionVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string PharmacistName { get; set; } = string.Empty;
        public List<SalesInvoiceItemDto> Items { get; set; } = new List<SalesInvoiceItemDto>();
    }
}
