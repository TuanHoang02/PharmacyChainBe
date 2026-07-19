namespace PharmacyChainBe.DTOs.Response
{
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalSalesCount { get; set; }
        public decimal TotalPurchaseExpense { get; set; }
        public decimal EstimatedProfit { get; set; }

        public List<DailyRevenueDto> DailyRevenue { get; set; } = new();
        public List<LowStockDto> LowStockMedicines { get; set; } = new();
        public List<ExpiringBatchDto> ExpiringBatches { get; set; } = new();
        public List<TopSellingDto> TopSellingMedicines { get; set; } = new();
        public List<BranchPerformanceDto>? BranchPerformance { get; set; }
    }

    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expense { get; set; }
    }

    public class LowStockDto
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string? BranchName { get; set; }
    }

    public class ExpiringBatchDto
    {
        public int MedicineBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string? BranchName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int RemainingQuantity { get; set; }
        public int DaysUntilExpiry { get; set; }
    }

    public class TopSellingDto
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenueGenerated { get; set; }
    }

    public class BranchPerformanceDto
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int SalesCount { get; set; }
    }
}
