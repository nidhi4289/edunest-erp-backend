using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace EduNestERP.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ITenantDataSourceProvider _dataSource;
        private readonly ILogger<DashboardRepository> _logger;

        public DashboardRepository(ITenantDataSourceProvider dataSource, ILogger<DashboardRepository> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
        }

        public async Task<DashboardSummary?> GetDashboardSummaryAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        total_active_students,
                        total_students,
                        student_active_percentage,
                        total_active_staff,
                        total_staff,
                        staff_active_percentage,
                        current_month_attendance_percentage,
                        school_days_this_month,
                        students_with_attendance,
                        today_attendance_percentage,
                        today_total_marked,
                        total_fee_collected,
                        overall_fee_collection_percentage,
                        students_with_fee_records,
                        current_month_fee_collected,
                        current_month_students_paid,
                        current_month_fee_transactions,
                        total_outstanding_fees,
                        students_with_outstanding_fees
                    FROM v_admin_dashboard_summary";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new DashboardSummary
                    {
                        TotalActiveStudents = reader.IsDBNull("total_active_students") ? 0 : reader.GetInt32("total_active_students"),
                        TotalStudents = reader.IsDBNull("total_students") ? 0 : reader.GetInt32("total_students"),
                        StudentActivePercentage = reader.IsDBNull("student_active_percentage") ? 0 : reader.GetDecimal("student_active_percentage"),
                        TotalActiveStaff = reader.IsDBNull("total_active_staff") ? 0 : reader.GetInt32("total_active_staff"),
                        TotalStaff = reader.IsDBNull("total_staff") ? 0 : reader.GetInt32("total_staff"),
                        StaffActivePercentage = reader.IsDBNull("staff_active_percentage") ? 0 : reader.GetDecimal("staff_active_percentage"),
                        CurrentMonthAttendancePercentage = reader.IsDBNull("current_month_attendance_percentage") ? 0 : reader.GetDecimal("current_month_attendance_percentage"),
                        SchoolDaysThisMonth = reader.IsDBNull("school_days_this_month") ? 0 : reader.GetInt32("school_days_this_month"),
                        StudentsWithAttendance = reader.IsDBNull("students_with_attendance") ? 0 : reader.GetInt32("students_with_attendance"),
                        TodayAttendancePercentage = reader.IsDBNull("today_attendance_percentage") ? 0 : reader.GetDecimal("today_attendance_percentage"),
                        TodayTotalMarked = reader.IsDBNull("today_total_marked") ? 0 : reader.GetInt32("today_total_marked"),
                        // Fee Statistics
                        TotalFeeCollected = reader.IsDBNull("total_fee_collected") ? 0 : reader.GetDecimal("total_fee_collected"),
                        OverallFeeCollectionPercentage = reader.IsDBNull("overall_fee_collection_percentage") ? 0 : reader.GetDecimal("overall_fee_collection_percentage"),
                        StudentsWithFeeRecords = reader.IsDBNull("students_with_fee_records") ? 0 : reader.GetInt64("students_with_fee_records"),
                        CurrentMonthFeeCollected = reader.IsDBNull("current_month_fee_collected") ? 0 : reader.GetDecimal("current_month_fee_collected"),
                        CurrentMonthStudentsPaid = reader.IsDBNull("current_month_students_paid") ? 0 : reader.GetInt64("current_month_students_paid"),
                        CurrentMonthFeeTransactions = reader.IsDBNull("current_month_fee_transactions") ? 0 : reader.GetInt64("current_month_fee_transactions"),
                        TotalOutstandingFees = reader.IsDBNull("total_outstanding_fees") ? 0 : reader.GetDecimal("total_outstanding_fees"),
                        StudentsWithOutstandingFees = reader.IsDBNull("students_with_outstanding_fees") ? 0 : reader.GetInt64("students_with_outstanding_fees")
                    };
                }

                _logger.LogWarning("GetDashboardSummaryAsync - No dashboard summary data found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDashboardSummaryAsync - Error getting dashboard summary");
                return null;
            }
        }

        public async Task<List<AttendanceTrend>> GetRecentAttendanceTrendsAsync(int days = 7)
        {
            try
            {
                const string sql = @"
                    SELECT date, attendance_percentage, total_marked, present_count, absent_count
                    FROM v_recent_attendance_trends
                    WHERE date >= CURRENT_DATE - INTERVAL '@days days'
                    ORDER BY date DESC";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql.Replace("@days", days.ToString()), connection);
                
                var trends = new List<AttendanceTrend>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    trends.Add(new AttendanceTrend
                    {
                        Date = reader.GetDateTime("date"),
                        AttendancePercentage = reader.IsDBNull("attendance_percentage") ? 0 : reader.GetDecimal("attendance_percentage"),
                        TotalMarked = reader.IsDBNull("total_marked") ? 0 : reader.GetInt32("total_marked"),
                        PresentCount = reader.IsDBNull("present_count") ? 0 : reader.GetInt32("present_count"),
                        AbsentCount = reader.IsDBNull("absent_count") ? 0 : reader.GetInt32("absent_count")
                    });
                }

                _logger.LogInformation("GetRecentAttendanceTrendsAsync - Retrieved {Count} attendance trends for {Days} days", 
                    trends.Count, days);
                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRecentAttendanceTrendsAsync - Error getting attendance trends for {Days} days", days);
                return new List<AttendanceTrend>();
            }
        }

        public async Task<List<ClassAttendanceSummary>> GetClassAttendanceSummaryAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        grade, section, class_id, total_attendance_records, 
                        present_count, absent_count, students_with_records, 
                        days_recorded, class_attendance_percentage
                    FROM v_class_attendance_summary
                    ORDER BY grade, section";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                
                var classSummaries = new List<ClassAttendanceSummary>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    classSummaries.Add(new ClassAttendanceSummary
                    {
                        Grade = reader.GetString("grade"),
                        Section = reader.IsDBNull("section") ? "" : reader.GetString("section"),
                        ClassId = reader.GetGuid("class_id"),
                        TotalAttendanceRecords = reader.IsDBNull("total_attendance_records") ? 0 : reader.GetInt32("total_attendance_records"),
                        PresentCount = reader.IsDBNull("present_count") ? 0 : reader.GetInt32("present_count"),
                        AbsentCount = reader.IsDBNull("absent_count") ? 0 : reader.GetInt32("absent_count"),
                        StudentsWithRecords = reader.IsDBNull("students_with_records") ? 0 : reader.GetInt32("students_with_records"),
                        DaysRecorded = reader.IsDBNull("days_recorded") ? 0 : reader.GetInt32("days_recorded"),
                        ClassAttendancePercentage = reader.IsDBNull("class_attendance_percentage") ? 0 : reader.GetDecimal("class_attendance_percentage")
                    });
                }

                _logger.LogInformation("GetClassAttendanceSummaryAsync - Retrieved {Count} class attendance summaries", classSummaries.Count);
                return classSummaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetClassAttendanceSummaryAsync - Error getting class attendance summary");
                return new List<ClassAttendanceSummary>();
            }
        }

        public async Task<AttendanceSummaryResult?> GetAttendanceSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today;

                const string sql = @"
                    SELECT * FROM get_attendance_summary(@StartDate, @EndDate)";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return new AttendanceSummaryResult
                    {
                        TotalSchoolDays = reader.IsDBNull("total_school_days") ? 0 : reader.GetInt64("total_school_days"),
                        AverageAttendancePercentage = reader.IsDBNull("average_attendance_percentage") ? 0 : reader.GetDecimal("average_attendance_percentage"),
                        TotalAttendanceRecords = reader.IsDBNull("total_attendance_records") ? 0 : reader.GetInt64("total_attendance_records"),
                        UniqueStudentsCount = reader.IsDBNull("unique_students_count") ? 0 : reader.GetInt64("unique_students_count")
                    };
                }

                _logger.LogInformation("GetAttendanceSummaryAsync - Retrieved attendance summary for {StartDate} to {EndDate}", 
                    startDate, endDate);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceSummaryAsync - Error getting attendance summary for {StartDate} to {EndDate}", 
                    startDate, endDate);
                return null;
            }
        }

        // Fee Statistics Methods
        public async Task<FeeStatistics?> GetFeeStatisticsAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        total_fee_records,
                        total_fee_collected,
                        total_fee_waived,
                        total_fee_due,
                        students_with_fee_records,
                        average_fee_collected,
                        fee_collection_percentage
                    FROM v_fee_statistics";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new FeeStatistics
                    {
                        TotalFeeRecords = reader.IsDBNull("total_fee_records") ? 0 : reader.GetInt64("total_fee_records"),
                        TotalFeeCollected = reader.IsDBNull("total_fee_collected") ? 0 : reader.GetDecimal("total_fee_collected"),
                        TotalFeeWaived = reader.IsDBNull("total_fee_waived") ? 0 : reader.GetDecimal("total_fee_waived"),
                        TotalFeeDue = reader.IsDBNull("total_fee_due") ? 0 : reader.GetDecimal("total_fee_due"),
                        StudentsWithFeeRecords = reader.IsDBNull("students_with_fee_records") ? 0 : reader.GetInt64("students_with_fee_records"),
                        AverageFeeCollected = reader.IsDBNull("average_fee_collected") ? 0 : reader.GetDecimal("average_fee_collected"),
                        FeeCollectionPercentage = reader.IsDBNull("fee_collection_percentage") ? 0 : reader.GetDecimal("fee_collection_percentage")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeStatisticsAsync - Error getting fee statistics");
                return null;
            }
        }

        public async Task<CurrentMonthFeeCollection?> GetCurrentMonthFeeCollectionAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        year,
                        month,
                        total_transactions,
                        total_collected_this_month,
                        total_waived_this_month,
                        unique_students_paid,
                        average_collection_per_transaction,
                        highest_collection,
                        lowest_collection
                    FROM v_current_month_fee_collection";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new CurrentMonthFeeCollection
                    {
                        Year = reader.IsDBNull("year") ? DateTime.Now.Year : (int)reader.GetDouble("year"),
                        Month = reader.IsDBNull("month") ? DateTime.Now.Month : (int)reader.GetDouble("month"),
                        TotalTransactions = reader.IsDBNull("total_transactions") ? 0 : reader.GetInt64("total_transactions"),
                        TotalCollectedThisMonth = reader.IsDBNull("total_collected_this_month") ? 0 : reader.GetDecimal("total_collected_this_month"),
                        TotalWaivedThisMonth = reader.IsDBNull("total_waived_this_month") ? 0 : reader.GetDecimal("total_waived_this_month"),
                        UniqueStudentsPaid = reader.IsDBNull("unique_students_paid") ? 0 : reader.GetInt64("unique_students_paid"),
                        AverageCollectionPerTransaction = reader.IsDBNull("average_collection_per_transaction") ? 0 : reader.GetDecimal("average_collection_per_transaction"),
                        HighestCollection = reader.IsDBNull("highest_collection") ? 0 : reader.GetDecimal("highest_collection"),
                        LowestCollection = reader.IsDBNull("lowest_collection") ? 0 : reader.GetDecimal("lowest_collection")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentMonthFeeCollectionAsync - Error getting current month fee collection");
                return null;
            }
        }

        public async Task<List<ClassFeeSummary>> GetClassFeeSummaryAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        grade, section, class_id, class_name,
                        total_fee_transactions, total_collected, total_waived,
                        students_paid, total_students_in_class,
                        average_fee_per_student, payment_completion_percentage
                    FROM v_class_fee_summary
                    ORDER BY grade, section";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                
                var classFees = new List<ClassFeeSummary>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    classFees.Add(new ClassFeeSummary
                    {
                        Grade = reader.GetString("grade"),
                        Section = reader.IsDBNull("section") ? "" : reader.GetString("section"),
                        ClassId = reader.GetGuid("class_id"),
                        ClassName = reader.IsDBNull("class_name") ? "" : reader.GetString("class_name"),
                        TotalFeeTransactions = reader.IsDBNull("total_fee_transactions") ? 0 : reader.GetInt64("total_fee_transactions"),
                        TotalCollected = reader.IsDBNull("total_collected") ? 0 : reader.GetDecimal("total_collected"),
                        TotalWaived = reader.IsDBNull("total_waived") ? 0 : reader.GetDecimal("total_waived"),
                        StudentsPaid = reader.IsDBNull("students_paid") ? 0 : reader.GetInt64("students_paid"),
                        TotalStudentsInClass = reader.IsDBNull("total_students_in_class") ? 0 : reader.GetInt64("total_students_in_class"),
                        AverageFeePerStudent = reader.IsDBNull("average_fee_per_student") ? 0 : reader.GetDecimal("average_fee_per_student"),
                        PaymentCompletionPercentage = reader.IsDBNull("payment_completion_percentage") ? 0 : reader.GetDecimal("payment_completion_percentage")
                    });
                }

                _logger.LogInformation("GetClassFeeSummaryAsync - Retrieved {Count} class fee summaries", classFees.Count);
                return classFees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetClassFeeSummaryAsync - Error getting class fee summary");
                return new List<ClassFeeSummary>();
            }
        }

        public async Task<List<FeeAdminSummary>> GetFeeAdminSummaryAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        grade, section, class_name, academic_year,
                        total_annual_fee, monthly_fee, is_active,
                        actual_collected, students_paid, total_students,
                        collection_vs_target_percentage
                    FROM v_fee_admin_summary
                    ORDER BY grade, section, academic_year";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                
                var feeAdminSummaries = new List<FeeAdminSummary>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    feeAdminSummaries.Add(new FeeAdminSummary
                    {
                        Grade = reader.GetString("grade"),
                        Section = reader.IsDBNull("section") ? "" : reader.GetString("section"),
                        ClassName = reader.IsDBNull("class_name") ? "" : reader.GetString("class_name"),
                        AcademicYear = reader.IsDBNull("academic_year") ? "" : reader.GetString("academic_year"),
                        TotalAnnualFee = reader.IsDBNull("total_annual_fee") ? 0 : reader.GetDecimal("total_annual_fee"),
                        MonthlyFee = reader.IsDBNull("monthly_fee") ? 0 : reader.GetDecimal("monthly_fee"),
                        IsActive = reader.IsDBNull("is_active") ? false : reader.GetBoolean("is_active"),
                        ActualCollected = reader.IsDBNull("actual_collected") ? 0 : reader.GetDecimal("actual_collected"),
                        StudentsPaid = reader.IsDBNull("students_paid") ? 0 : reader.GetInt64("students_paid"),
                        TotalStudents = reader.IsDBNull("total_students") ? 0 : reader.GetInt64("total_students"),
                        CollectionVsTargetPercentage = reader.IsDBNull("collection_vs_target_percentage") ? 0 : reader.GetDecimal("collection_vs_target_percentage")
                    });
                }

                _logger.LogInformation("GetFeeAdminSummaryAsync - Retrieved {Count} fee admin summaries", feeAdminSummaries.Count);
                return feeAdminSummaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeAdminSummaryAsync - Error getting fee admin summary");
                return new List<FeeAdminSummary>();
            }
        }

        public async Task<List<FeeCollectionTrend>> GetRecentFeeCollectionTrendsAsync(int days = 7)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        collection_date, total_collected, transactions_count,
                        unique_students, average_per_transaction
                    FROM v_recent_fee_trends
                    WHERE collection_date >= CURRENT_DATE - INTERVAL '@days days'
                    ORDER BY collection_date DESC";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql.Replace("@days", days.ToString()), connection);
                
                var trends = new List<FeeCollectionTrend>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    trends.Add(new FeeCollectionTrend
                    {
                        CollectionDate = reader.GetDateTime("collection_date"),
                        TotalCollected = reader.IsDBNull("total_collected") ? 0 : reader.GetDecimal("total_collected"),
                        TransactionsCount = reader.IsDBNull("transactions_count") ? 0 : reader.GetInt64("transactions_count"),
                        UniqueStudents = reader.IsDBNull("unique_students") ? 0 : reader.GetInt64("unique_students"),
                        AveragePerTransaction = reader.IsDBNull("average_per_transaction") ? 0 : reader.GetDecimal("average_per_transaction")
                    });
                }

                _logger.LogInformation("GetRecentFeeCollectionTrendsAsync - Retrieved {Count} fee collection trends for {Days} days", 
                    trends.Count, days);
                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRecentFeeCollectionTrendsAsync - Error getting fee collection trends for {Days} days", days);
                return new List<FeeCollectionTrend>();
            }
        }

        public async Task<List<OutstandingFeesSummary>> GetOutstandingFeesSummaryAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        grade, section, class_name, total_students,
                        students_paid, students_not_paid, expected_fee_per_student,
                        total_expected_collection, actual_collection, outstanding_amount
                    FROM v_outstanding_fees_summary
                    ORDER BY grade, section";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                
                var outstandingFees = new List<OutstandingFeesSummary>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    outstandingFees.Add(new OutstandingFeesSummary
                    {
                        Grade = reader.GetString("grade"),
                        Section = reader.IsDBNull("section") ? "" : reader.GetString("section"),
                        ClassName = reader.IsDBNull("class_name") ? "" : reader.GetString("class_name"),
                        TotalStudents = reader.IsDBNull("total_students") ? 0 : reader.GetInt64("total_students"),
                        StudentsPaid = reader.IsDBNull("students_paid") ? 0 : reader.GetInt64("students_paid"),
                        StudentsNotPaid = reader.IsDBNull("students_not_paid") ? 0 : reader.GetInt64("students_not_paid"),
                        ExpectedFeePerStudent = reader.IsDBNull("expected_fee_per_student") ? 0 : reader.GetDecimal("expected_fee_per_student"),
                        TotalExpectedCollection = reader.IsDBNull("total_expected_collection") ? 0 : reader.GetDecimal("total_expected_collection"),
                        ActualCollection = reader.IsDBNull("actual_collection") ? 0 : reader.GetDecimal("actual_collection"),
                        OutstandingAmount = reader.IsDBNull("outstanding_amount") ? 0 : reader.GetDecimal("outstanding_amount")
                    });
                }

                _logger.LogInformation("GetOutstandingFeesSummaryAsync - Retrieved {Count} outstanding fees summaries", outstandingFees.Count);
                return outstandingFees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOutstandingFeesSummaryAsync - Error getting outstanding fees summary");
                return new List<OutstandingFeesSummary>();
            }
        }

        public async Task<FeeCollectionSummaryResult?> GetFeeCollectionSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today;

                const string sql = @"
                    SELECT * FROM get_fee_collection_summary(@StartDate, @EndDate)";

                await using var connection = await _dataSource.OpenConnectionAsync();
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return new FeeCollectionSummaryResult
                    {
                        TotalCollectionDays = reader.IsDBNull("total_collection_days") ? 0 : reader.GetInt64("total_collection_days"),
                        TotalAmountCollected = reader.IsDBNull("total_amount_collected") ? 0 : reader.GetDecimal("total_amount_collected"),
                        TotalTransactions = reader.IsDBNull("total_transactions") ? 0 : reader.GetInt64("total_transactions"),
                        UniqueStudentsPaid = reader.IsDBNull("unique_students_paid") ? 0 : reader.GetInt64("unique_students_paid"),
                        AverageDailyCollection = reader.IsDBNull("average_daily_collection") ? 0 : reader.GetDecimal("average_daily_collection"),
                        AveragePerTransaction = reader.IsDBNull("average_per_transaction") ? 0 : reader.GetDecimal("average_per_transaction")
                    };
                }

                _logger.LogInformation("GetFeeCollectionSummaryAsync - Retrieved fee collection summary for {StartDate} to {EndDate}", 
                    startDate, endDate);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeeCollectionSummaryAsync - Error getting fee collection summary for {StartDate} to {EndDate}", 
                    startDate, endDate);
                return null;
            }
        }
    }
}