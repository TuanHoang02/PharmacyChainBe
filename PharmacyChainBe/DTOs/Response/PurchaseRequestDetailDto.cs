namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseRequestDetailDto
    {
        public int PurchaseRequestDetailID { get; set; }
        public int PurchaseRequestID { get; set; }
        public int MedicineID { get; set; }
        public string? MedicineName { get; set; }
        public string? Unit { get; set; }
        public int CurrentStock { get; set; }
        public int RequestedQuantity { get; set; }
    }
}
