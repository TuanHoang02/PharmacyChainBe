namespace PharmacyChainBe.DTOs.Response
{
    public class BranchPerformanceResponse
    {
        public decimal TotalSales { get; set; }
        public int TotalInvoices { get; set; }
        public int LowStockMedicines { get; set; }
        public List<BranchRankingResponse> BranchRanking { get; set; } = new List<BranchRankingResponse>();
    }



    public class BranchRankingResponse
    {
        public int Rank { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int TotalInvoices { get; set; }
        public int LowStockMedicines { get; set; }
    }
}
