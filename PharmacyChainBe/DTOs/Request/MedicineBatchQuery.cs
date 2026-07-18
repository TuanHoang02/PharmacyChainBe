namespace PharmacyChainBe.DTOs.Request
{
    public class MedicineBatchQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? MedicineID { get; set; }
        public int? PurchaseOrderID { get; set; }
        public string? BatchNumber { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = true;
    }
}
