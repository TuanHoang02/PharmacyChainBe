namespace PharmacyChainBe.DTOs.Response
{
    public class MedicineBatchDto
    {
        public int MedicineBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public DateTime ManufacturingDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int ReceivedQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
