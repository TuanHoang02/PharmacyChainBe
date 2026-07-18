using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Services.Interfaces;
using PharmacyChainBe.Repositories.Interfaces;
using System.Globalization;

namespace PharmacyChainBe.Services.Implementations
{
    public class BranchPerformanceService : IBranchPerformanceService
    {
        private readonly IBranchPerformanceRepository _repository;

        public BranchPerformanceService(IBranchPerformanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<BranchPerformanceResponse> GetPerformanceDataAsync(int? branchId, string period, DateTime? customStartDate = null, DateTime? customEndDate = null)
        {
            var now = DateTime.UtcNow;
            DateTime startDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = now.Date;
                    break;
                case "this week":
                    // Assuming week starts on Monday
                    int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = now.AddDays(-1 * diff).Date;
                    break;
                case "this month":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
                case "custom":
                    if (customStartDate.HasValue && customEndDate.HasValue)
                    {
                        startDate = customStartDate.Value.Date;
                        now = customEndDate.Value.Date.AddDays(1).AddTicks(-1); // End of the selected day
                    }
                    else
                    {
                        // Fallback if dates are missing but custom is selected
                        startDate = now.Date.AddDays(-7); 
                    }
                    break;
                default:
                    // default to this month
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
            }

            var totalSales = await _repository.GetTotalSalesAsync(startDate, now, branchId);
            var totalInvoices = await _repository.GetTotalInvoicesAsync(startDate, now, branchId);
            var lowStockCount = await _repository.GetLowStockMedicinesCountAsync(branchId);

            // Get Branches for Ranking and other ByBranch charts
            var branches = await _repository.GetActiveBranchesAsync();
            
            // Optimize: fetch all branches' totals at once instead of in a loop
            var branchSalesDict = await _repository.GetTotalSalesByBranchAsync(startDate, now);
            var branchInvoicesDict = await _repository.GetTotalInvoicesByBranchAsync(startDate, now);
            var branchLowStockDict = await _repository.GetLowStockMedicinesCountByBranchAsync();

            var branchRanking = new List<BranchRankingResponse>();

            foreach (var branch in branches)
            {
                var branchSales = branchSalesDict.ContainsKey(branch.BranchID) ? branchSalesDict[branch.BranchID] : 0;
                var branchInvoices = branchInvoicesDict.ContainsKey(branch.BranchID) ? branchInvoicesDict[branch.BranchID] : 0;
                var branchLowStock = branchLowStockDict.ContainsKey(branch.BranchID) ? branchLowStockDict[branch.BranchID] : 0;

                branchRanking.Add(new BranchRankingResponse
                {
                    BranchName = branch.BranchName,
                    TotalSales = branchSales,
                    TotalInvoices = branchInvoices,
                    LowStockMedicines = branchLowStock
                });
            }

            // Sort Ranking by Sales Desc
            branchRanking = branchRanking.OrderByDescending(b => b.TotalSales).ToList();
            for (int i = 0; i < branchRanking.Count; i++)
            {
                branchRanking[i].Rank = i + 1;
            }

            // If a specific branch is selected, adjust the main KPI to reflect that branch, else aggregate
            if (branchId.HasValue && branchId.Value > 0)
            {
                var targetBranch = branchRanking.FirstOrDefault(b => b.BranchName == branches.FirstOrDefault(br => br.BranchID == branchId.Value)?.BranchName);
                if (targetBranch != null)
                {
                    totalSales = targetBranch.TotalSales;
                    totalInvoices = targetBranch.TotalInvoices;
                    lowStockCount = targetBranch.LowStockMedicines;
                }
            }
            else
            {
                // Aggregate overall
                totalSales = branchRanking.Sum(b => b.TotalSales);
                totalInvoices = branchRanking.Sum(b => b.TotalInvoices);
                lowStockCount = branchRanking.Sum(b => b.LowStockMedicines);
            }

            return new BranchPerformanceResponse
            {
                TotalSales = totalSales,
                TotalInvoices = totalInvoices,
                LowStockMedicines = lowStockCount,
                BranchRanking = branchRanking
            };
        }
    }
}
