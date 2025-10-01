using Microsoft.AspNetCore.Mvc;
using EduNestERP.Application.Interfaces;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace EduNestERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;
        private readonly IMapper _mapper;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger, IMapper mapper)
        {
            _dashboardService = dashboardService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get complete dashboard data including summary, trends, and class attendance
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                _logger.LogInformation("GetDashboard - Request received for complete dashboard data");
                
                var summaryTask = _dashboardService.GetDashboardSummaryAsync();
                var trendsTask = _dashboardService.GetRecentAttendanceTrendsAsync();
                var classAttendanceTask = _dashboardService.GetClassAttendanceSummaryAsync();
                var feeCollectionTrendsTask = _dashboardService.GetRecentFeeCollectionTrendsAsync();
                var classFeesSummaryTask = _dashboardService.GetClassFeeSummaryAsync();
                var outstandingFeesTask = _dashboardService.GetOutstandingFeesSummaryAsync();

                await Task.WhenAll(summaryTask, trendsTask, classAttendanceTask, 
                    feeCollectionTrendsTask, classFeesSummaryTask, outstandingFeesTask);

                var summary = await summaryTask;
                var trends = await trendsTask;
                var classAttendance = await classAttendanceTask;
                var feeCollectionTrends = await feeCollectionTrendsTask;
                var classFeesSummary = await classFeesSummaryTask;
                var outstandingFees = await outstandingFeesTask;

                var dashboard = new DashboardDto
                {
                    Summary = summary != null ? _mapper.Map<DashboardSummaryDto>(summary) : new DashboardSummaryDto(),
                    RecentTrends = _mapper.Map<List<AttendanceTrendDto>>(trends),
                    ClassAttendance = _mapper.Map<List<ClassAttendanceDto>>(classAttendance),
                    FeeCollectionTrends = _mapper.Map<List<FeeCollectionTrendDto>>(feeCollectionTrends),
                    ClassFeesSummary = _mapper.Map<List<ClassFeeSummaryDto>>(classFeesSummary),
                    OutstandingFees = _mapper.Map<List<OutstandingFeesDto>>(outstandingFees)
                };
                
                _logger.LogInformation("GetDashboard - Dashboard data retrieved successfully");
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDashboard - Exception occurred while retrieving dashboard data");
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard data" });
            }
        }

        /// <summary>
        /// Get dashboard summary with key metrics
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                _logger.LogInformation("GetDashboardSummary - Request received for dashboard summary");
                
                var summary = await _dashboardService.GetDashboardSummaryAsync();
                var summaryDto = summary != null ? _mapper.Map<DashboardSummaryDto>(summary) : new DashboardSummaryDto();
                
                _logger.LogInformation("GetDashboardSummary - Dashboard summary retrieved successfully");
                return Ok(summaryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDashboardSummary - Exception occurred while retrieving dashboard summary");
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard summary" });
            }
        }

        /// <summary>
        /// Get recent attendance trends
        /// </summary>
        /// <param name="days">Number of days to look back (default: 7)</param>
        [HttpGet("attendance-trends")]
        public async Task<IActionResult> GetAttendanceTrends([FromQuery] int days = 7)
        {
            try
            {
                if (days < 1 || days > 365)
                {
                    return BadRequest(new { message = "Days parameter must be between 1 and 365" });
                }

                _logger.LogInformation("GetAttendanceTrends - Request received for {Days} days", days);
                
                var trends = await _dashboardService.GetRecentAttendanceTrendsAsync(days);
                var trendsDto = _mapper.Map<List<AttendanceTrendDto>>(trends);
                
                _logger.LogInformation("GetAttendanceTrends - Attendance trends retrieved successfully for {Days} days", days);
                return Ok(trendsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceTrends - Exception occurred while retrieving attendance trends for {Days} days", days);
                return StatusCode(500, new { message = "An error occurred while retrieving attendance trends" });
            }
        }

        /// <summary>
        /// Get class-wise attendance summary for current month
        /// </summary>
        [HttpGet("class-attendance")]
        public async Task<IActionResult> GetClassAttendance()
        {
            try
            {
                _logger.LogInformation("GetClassAttendance - Request received for class attendance summary");
                
                var classAttendance = await _dashboardService.GetClassAttendanceSummaryAsync();
                var classAttendanceDto = _mapper.Map<List<ClassAttendanceDto>>(classAttendance);
                
                _logger.LogInformation("GetClassAttendance - Class attendance summary retrieved successfully");
                return Ok(classAttendanceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetClassAttendance - Exception occurred while retrieving class attendance summary");
                return StatusCode(500, new { message = "An error occurred while retrieving class attendance summary" });
            }
        }

        /// <summary>
        /// Get attendance summary for a specific date range
        /// </summary>
        /// <param name="startDate">Start date (default: 30 days ago)</param>
        /// <param name="endDate">End date (default: today)</param>
        [HttpGet("attendance-summary")]
        public async Task<IActionResult> GetAttendanceSummary([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today;

                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be later than end date" });
                }

                if ((endDate - startDate)?.Days > 365)
                {
                    return BadRequest(new { message = "Date range cannot exceed 365 days" });
                }

                _logger.LogInformation("GetAttendanceSummary - Request received for date range {StartDate} to {EndDate}", 
                    startDate, endDate);
                
                var summary = await _dashboardService.GetAttendanceSummaryAsync(startDate, endDate);
                var summaryDto = summary != null ? _mapper.Map<AttendanceSummaryDto>(summary) : new AttendanceSummaryDto();
                
                _logger.LogInformation("GetAttendanceSummary - Attendance summary retrieved successfully for date range");
                return Ok(summaryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceSummary - Exception occurred while retrieving attendance summary for {StartDate} to {EndDate}", 
                    startDate, endDate);
                return StatusCode(500, new { message = "An error occurred while retrieving attendance summary" });
            }
        }

        // Fee Statistics Endpoints

        /// <summary>
        /// Get overall fee statistics
        /// </summary>
        [HttpGet("fee-statistics")]
        public async Task<IActionResult> GetFeeStatistics()
        {
            try
            {
                _logger.LogInformation("GetFeeStatistics - Request received for fee statistics");
                
                var feeStats = await _dashboardService.GetFeeStatisticsAsync();
                var feeStatsDto = feeStats != null ? _mapper.Map<FeeStatisticsDto>(feeStats) : new FeeStatisticsDto();
                
                _logger.LogInformation("GetFeeStatistics - Fee statistics retrieved successfully");
                return Ok(feeStatsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeStatistics - Exception occurred while retrieving fee statistics");
                return StatusCode(500, new { message = "An error occurred while retrieving fee statistics" });
            }
        }

        /// <summary>
        /// Get fee collection trends for recent days
        /// </summary>
        /// <param name="days">Number of days to look back (default: 7)</param>
        [HttpGet("fee-collection-trends")]
        public async Task<IActionResult> GetFeeCollectionTrends([FromQuery] int days = 7)
        {
            try
            {
                if (days < 1 || days > 365)
                {
                    return BadRequest(new { message = "Days parameter must be between 1 and 365" });
                }

                _logger.LogInformation("GetFeeCollectionTrends - Request received for {Days} days", days);
                
                var trends = await _dashboardService.GetRecentFeeCollectionTrendsAsync(days);
                var trendsDto = _mapper.Map<List<FeeCollectionTrendDto>>(trends);
                
                _logger.LogInformation("GetFeeCollectionTrends - Fee collection trends retrieved successfully for {Days} days", days);
                return Ok(trendsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeCollectionTrends - Exception occurred while retrieving fee collection trends for {Days} days", days);
                return StatusCode(500, new { message = "An error occurred while retrieving fee collection trends" });
            }
        }

        /// <summary>
        /// Get class-wise fee summary
        /// </summary>
        [HttpGet("class-fees")]
        public async Task<IActionResult> GetClassFees()
        {
            try
            {
                _logger.LogInformation("GetClassFees - Request received for class fee summary");
                
                var classFees = await _dashboardService.GetClassFeeSummaryAsync();
                var classFeesDto = _mapper.Map<List<ClassFeeSummaryDto>>(classFees);
                
                _logger.LogInformation("GetClassFees - Class fee summary retrieved successfully");
                return Ok(classFeesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetClassFees - Exception occurred while retrieving class fee summary");
                return StatusCode(500, new { message = "An error occurred while retrieving class fee summary" });
            }
        }

        /// <summary>
        /// Get outstanding fees summary by class
        /// </summary>
        [HttpGet("outstanding-fees")]
        public async Task<IActionResult> GetOutstandingFees()
        {
            try
            {
                _logger.LogInformation("GetOutstandingFees - Request received for outstanding fees summary");
                
                var outstandingFees = await _dashboardService.GetOutstandingFeesSummaryAsync();
                var outstandingFeesDto = _mapper.Map<List<OutstandingFeesDto>>(outstandingFees);
                
                _logger.LogInformation("GetOutstandingFees - Outstanding fees summary retrieved successfully");
                return Ok(outstandingFeesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOutstandingFees - Exception occurred while retrieving outstanding fees summary");
                return StatusCode(500, new { message = "An error occurred while retrieving outstanding fees summary" });
            }
        }

        /// <summary>
        /// Get fee collection summary for a specific date range
        /// </summary>
        /// <param name="startDate">Start date (default: 30 days ago)</param>
        /// <param name="endDate">End date (default: today)</param>
        [HttpGet("fee-collection-summary")]
        public async Task<IActionResult> GetFeeCollectionSummary([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today;

                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be later than end date" });
                }

                if ((endDate - startDate)?.Days > 365)
                {
                    return BadRequest(new { message = "Date range cannot exceed 365 days" });
                }

                _logger.LogInformation("GetFeeCollectionSummary - Request received for date range {StartDate} to {EndDate}", 
                    startDate, endDate);
                
                var summary = await _dashboardService.GetFeeCollectionSummaryAsync(startDate, endDate);
                var summaryDto = summary != null ? _mapper.Map<FeeCollectionSummaryDto>(summary) : new FeeCollectionSummaryDto();
                
                _logger.LogInformation("GetFeeCollectionSummary - Fee collection summary retrieved successfully for date range");
                return Ok(summaryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeCollectionSummary - Exception occurred while retrieving fee collection summary for {StartDate} to {EndDate}", 
                    startDate, endDate);
                return StatusCode(500, new { message = "An error occurred while retrieving fee collection summary" });
            }
        }
    }
}