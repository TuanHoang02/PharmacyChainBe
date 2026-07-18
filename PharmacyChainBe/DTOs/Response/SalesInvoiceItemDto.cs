namespace PharmacyChainBe.DTOs.Response
{
    public class SalesInvoiceItemDto
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
