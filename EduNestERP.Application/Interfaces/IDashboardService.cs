using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Interfaces
{
    public interface IDashboardService
    {
        // Attendance related methods
        Task<DashboardSummary?> GetDashboardSummaryAsync();
        Task<List<AttendanceTrend>> GetRecentAttendanceTrendsAsync(int days = 7);
        Task<List<ClassAttendanceSummary>> GetClassAttendanceSummaryAsync();
        Task<AttendanceSummaryResult?> GetAttendanceSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        
        // Fee statistics methods
        Task<FeeStatistics?> GetFeeStatisticsAsync();
        Task<CurrentMonthFeeCollection?> GetCurrentMonthFeeCollectionAsync();
        Task<List<ClassFeeSummary>> GetClassFeeSummaryAsync();
        Task<List<FeeAdminSummary>> GetFeeAdminSummaryAsync();
        Task<List<FeeCollectionTrend>> GetRecentFeeCollectionTrendsAsync(int days = 7);
        Task<List<OutstandingFeesSummary>> GetOutstandingFeesSummaryAsync();
        Task<FeeCollectionSummaryResult?> GetFeeCollectionSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}