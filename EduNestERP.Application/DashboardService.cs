using EduNestERP.Application.Interfaces;
using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;
using Microsoft.Extensions.Logging;

namespace EduNestERP.Application
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IDashboardRepository dashboardRepository, ILogger<DashboardService> logger)
        {
            _dashboardRepository = dashboardRepository;
            _logger = logger;
        }

        public async Task<DashboardSummary?> GetDashboardSummaryAsync()
        {
            try
            {
                _logger.LogInformation("GetDashboardSummaryAsync - Fetching dashboard summary");

                var summary = await _dashboardRepository.GetDashboardSummaryAsync();
                
                _logger.LogInformation("GetDashboardSummaryAsync - Dashboard summary retrieved successfully");
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDashboardSummaryAsync - Error retrieving dashboard summary");
                throw;
            }
        }

        public async Task<List<AttendanceTrend>> GetRecentAttendanceTrendsAsync(int days = 7)
        {
            try
            {
                _logger.LogInformation("GetRecentAttendanceTrendsAsync - Fetching attendance trends for {Days} days", days);

                var trends = await _dashboardRepository.GetRecentAttendanceTrendsAsync(days);
                
                _logger.LogInformation("GetRecentAttendanceTrendsAsync - Retrieved {Count} attendance trends", trends.Count);
                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRecentAttendanceTrendsAsync - Error retrieving attendance trends for {Days} days", days);
                throw;
            }
        }

        public async Task<List<ClassAttendanceSummary>> GetClassAttendanceSummaryAsync()
        {
            try
            {
                _logger.LogInformation("GetClassAttendanceSummaryAsync - Fetching class attendance summary");

                var classSummaries = await _dashboardRepository.GetClassAttendanceSummaryAsync();
                
                _logger.LogInformation("GetClassAttendanceSummaryAsync - Retrieved {Count} class attendance summaries", classSummaries.Count);
                return classSummaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetClassAttendanceSummaryAsync - Error retrieving class attendance summary");
                throw;
            }
        }

        public async Task<AttendanceSummaryResult?> GetAttendanceSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today;

                _logger.LogInformation("GetAttendanceSummaryAsync - Fetching attendance summary from {StartDate} to {EndDate}", 
                    startDate, endDate);

                var summary = await _dashboardRepository.GetAttendanceSummaryAsync(startDate, endDate);
                
                _logger.LogInformation("GetAttendanceSummaryAsync - Attendance summary retrieved successfully");
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceSummaryAsync - Error retrieving attendance summary from {StartDate} to {EndDate}", 
                    startDate, endDate);
                throw;
            }
        }

        // Fee Statistics Methods
        public async Task<FeeStatistics?> GetFeeStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("GetFeeStatisticsAsync - Fetching fee statistics");

                var feeStats = await _dashboardRepository.GetFeeStatisticsAsync();
                
                _logger.LogInformation("GetFeeStatisticsAsync - Fee statistics retrieved successfully");
                return feeStats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeStatisticsAsync - Error retrieving fee statistics");
                throw;
            }
        }

        public async Task<CurrentMonthFeeCollection?> GetCurrentMonthFeeCollectionAsync()
        {
            try
            {
                _logger.LogInformation("GetCurrentMonthFeeCollectionAsync - Fetching current month fee collection");

                var feeCollection = await _dashboardRepository.GetCurrentMonthFeeCollectionAsync();
                
                _logger.LogInformation("GetCurrentMonthFeeCollectionAsync - Current month fee collection retrieved successfully");
                return feeCollection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentMonthFeeCollectionAsync - Error retrieving current month fee collection");
                throw;
            }
        }

        public async Task<List<ClassFeeSummary>> GetClassFeeSummaryAsync()
        {
            try
            {
                _logger.LogInformation("GetClassFeeSummaryAsync - Fetching class fee summary");

                var classFees = await _dashboardRepository.GetClassFeeSummaryAsync();
                
                _logger.LogInformation("GetClassFeeSummaryAsync - Retrieved {Count} class fee summaries", classFees.Count);
                return classFees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetClassFeeSummaryAsync - Error retrieving class fee summary");
                throw;
            }
        }

        public async Task<List<FeeAdminSummary>> GetFeeAdminSummaryAsync()
        {
            try
            {
                _logger.LogInformation("GetFeeAdminSummaryAsync - Fetching fee admin summary");

                var feeAdminSummaries = await _dashboardRepository.GetFeeAdminSummaryAsync();
                
                _logger.LogInformation("GetFeeAdminSummaryAsync - Retrieved {Count} fee admin summaries", feeAdminSummaries.Count);
                return feeAdminSummaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminSummaryAsync - Error retrieving fee admin summary");
                throw;
            }
        }

        public async Task<List<FeeCollectionTrend>> GetRecentFeeCollectionTrendsAsync(int days = 7)
        {
            try
            {
                _logger.LogInformation("GetRecentFeeCollectionTrendsAsync - Fetching fee collection trends for {Days} days", days);

                var trends = await _dashboardRepository.GetRecentFeeCollectionTrendsAsync(days);
                
                _logger.LogInformation("GetRecentFeeCollectionTrendsAsync - Retrieved {Count} fee collection trends", trends.Count);
                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRecentFeeCollectionTrendsAsync - Error retrieving fee collection trends for {Days} days", days);
                throw;
            }
        }

        public async Task<List<OutstandingFeesSummary>> GetOutstandingFeesSummaryAsync()
        {
            try
            {
                _logger.LogInformation("GetOutstandingFeesSummaryAsync - Fetching outstanding fees summary");

                var outstandingFees = await _dashboardRepository.GetOutstandingFeesSummaryAsync();
                
                _logger.LogInformation("GetOutstandingFeesSummaryAsync - Retrieved {Count} outstanding fees summaries", outstandingFees.Count);
                return outstandingFees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOutstandingFeesSummaryAsync - Error retrieving outstanding fees summary");
                throw;
            }
        }

        public async Task<FeeCollectionSummaryResult?> GetFeeCollectionSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today;

                _logger.LogInformation("GetFeeCollectionSummaryAsync - Fetching fee collection summary from {StartDate} to {EndDate}", 
                    startDate, endDate);

                var summary = await _dashboardRepository.GetFeeCollectionSummaryAsync(startDate, endDate);
                
                _logger.LogInformation("GetFeeCollectionSummaryAsync - Fee collection summary retrieved successfully");
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeCollectionSummaryAsync - Error retrieving fee collection summary from {StartDate} to {EndDate}", 
                    startDate, endDate);
                throw;
            }
        }
    }
}