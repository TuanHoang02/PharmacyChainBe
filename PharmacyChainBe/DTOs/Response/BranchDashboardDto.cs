namespace PharmacyChainBe.DTOs.Response
{
    public class BranchDashboardDto
    {
        public string BranchName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public SalesSummaryDto Sales { get; set; } = new();
        public InventoryStatusDto Inventory { get; set; } = new();
        public List<LowStockMedicineDto> LowStockMedicines { get; set; } = new();
        public int PendingPurchaseOrders { get; set; }
    }

    public class SalesSummaryDto
    {
        public decimal TodayRevenue { get; set; }
        public decimal WeekRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public int TodayInvoices { get; set; }
        public int PendingInvoices { get; set; }
    }

    public class InventoryStatusDto
    {
        public int TotalSkus { get; set; }
        public int TotalBatches { get; set; }
        public int NearExpiryBatches { get; set; }
        public int ExpiredBatches { get; set; }
        public decimal TotalStockValue { get; set; }
    }

    public class LowStockMedicineDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
    }
}
