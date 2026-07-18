namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseOrderItemDto
    {
        public int PurchaseOrderDetailID { get; set; }
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
