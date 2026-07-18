namespace PharmacyChainBe.DTOs.Response
{
    public class MedicineDto
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public string? Unit { get; set; }
        public bool RequiresPrescription { get; set; }
        public bool IsActive { get; set; }
    }
}
