namespace PharmacyChainBe.DTOs.Response
{
    public class BranchReportResponseDto
    {
        public string BranchName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public ReportSummaryDto Summary { get; set; } = new();
        public List<SalesReportDetailDto> SalesDetails { get; set; } = new();
        public List<InventoryReportDetailDto> InventoryDetails { get; set; } = new();
    }

    public class ReportSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }

        // Inventory-specific
        public decimal TotalStockValue { get; set; }
        public int TotalSkus { get; set; }
        public int LowStockCount { get; set; }
        public int NearExpiryCount { get; set; }
        public int ExpiredCount { get; set; }
    }

    public class SalesReportDetailDto
    {
        public string InvoiceCode { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? CustomerName { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class InventoryReportDetailDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal StockValue { get; set; }
        public int TotalBatches { get; set; }
        public DateTime? NearestExpiryDate { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }
}
