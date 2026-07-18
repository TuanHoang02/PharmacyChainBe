namespace PharmacyChainBe.DTOs.Response
{
    public class BranchPerformanceResponse
    {
        public decimal TotalSales { get; set; }
        public double InventoryTurnover { get; set; }
        public int ExpiredMedicines { get; set; }
        public double StaffPerformanceScore { get; set; }
        
        public List<SalesTrendResponse> SalesTrend { get; set; } = new List<SalesTrendResponse>();
        public List<ExpiredMedicineByBranchResponse> ExpiredMedicinesByBranch { get; set; } = new List<ExpiredMedicineByBranchResponse>();
        public List<InventoryTurnoverByBranchResponse> InventoryTurnoverByBranch { get; set; } = new List<InventoryTurnoverByBranchResponse>();
        public List<BranchRankingResponse> BranchRanking { get; set; } = new List<BranchRankingResponse>();
    }

    public class SalesTrendResponse
    {
        public string DateLabel { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
    }

    public class ExpiredMedicineByBranchResponse
    {
        public string BranchName { get; set; } = string.Empty;
        public int ExpiredCount { get; set; }
    }

    public class InventoryTurnoverByBranchResponse
    {
        public string BranchName { get; set; } = string.Empty;
        public double TurnoverRate { get; set; }
    }

    public class BranchRankingResponse
    {
        public int Rank { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public double InventoryTurnover { get; set; }
        public int ExpiredMedicines { get; set; }
        public double StaffScore { get; set; }
    }
}
