namespace PharmacyChainBe.DTOs.Response
{
    public class InventoryResponseDto
    {
        public int InventoryId { get; set; }
        public int BranchId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsLowStock { get; set; }
        public decimal SellingPrice { get; set; }
        public bool RequiresPrescription { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
