using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(
            int? branchId, 
            DateTime? startDate, 
            DateTime? endDate, 
            int? currentUserBranchId, 
            string currentUserRole)
        {
            // 1. Phân quyền truy cập dữ liệu
            if (currentUserRole != "OperationsManager")
            {
                if (currentUserRole == "BranchManager")
                {
                    // BranchManager chỉ được xem chi nhánh của mình, ghi đè tham số branchId
                    branchId = currentUserBranchId;
                }
                else
                {
                    throw new ApiException("Bạn không có quyền truy cập dữ liệu Dashboard.", 403);
                }
            }

            // 2. Thiết lập thời gian mặc định (30 ngày gần nhất nếu để trống)
            var start = startDate ?? DateTime.UtcNow.Date.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            // Đảm bảo thời gian hợp lý
            if (start > end)
            {
                throw new ApiException("Ngày bắt đầu không được lớn hơn ngày kết thúc.", 400);
            }

            // 3. Truy vấn các số liệu thống kê
            var totalRevenue = await _dashboardRepository.GetTotalRevenueAsync(branchId, start, end);
            var totalSalesCount = await _dashboardRepository.GetSalesCountAsync(branchId, start, end);
            var totalPurchaseExpense = await _dashboardRepository.GetTotalPurchaseExpenseAsync(branchId, start, end);
            var dailyStats = await _dashboardRepository.GetDailyStatisticsAsync(branchId, start, end);
            var lowStock = await _dashboardRepository.GetLowStockMedicinesAsync(branchId);
            var expiring = await _dashboardRepository.GetExpiringBatchesAsync(branchId, 30); // 30 ngày tới
            var topSelling = await _dashboardRepository.GetTopSellingMedicinesAsync(branchId, start, end, 5); // Lấy top 5

            List<BranchPerformanceDto>? branchPerformance = null;
            
            // Chỉ hiển thị so sánh hiệu suất chi nhánh nếu là OpsManager và đang xem toàn chuỗi (branchId == null)
            if (!branchId.HasValue && currentUserRole == "OperationsManager")
            {
                branchPerformance = await _dashboardRepository.GetBranchPerformanceAsync(start, end);
            }

            return new DashboardSummaryDto
            {
                TotalRevenue = totalRevenue,
                TotalSalesCount = totalSalesCount,
                TotalPurchaseExpense = totalPurchaseExpense,
                EstimatedProfit = totalRevenue - totalPurchaseExpense,
                DailyRevenue = dailyStats,
                LowStockMedicines = lowStock,
                ExpiringBatches = expiring,
                TopSellingMedicines = topSelling,
                BranchPerformance = branchPerformance
            };
        }
    }
}
