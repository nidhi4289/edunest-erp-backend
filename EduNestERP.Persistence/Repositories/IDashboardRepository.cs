using EduNestERP.Persistence.Entities;

namespace EduNestERP.Persistence.Repositories
{
    public interface IDashboardRepository
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

    public class DashboardSummary
    {
        public int TotalActiveStudents { get; set; }
        public int TotalStudents { get; set; }
        public decimal StudentActivePercentage { get; set; }
        public int TotalActiveStaff { get; set; }
        public int TotalStaff { get; set; }
        public decimal StaffActivePercentage { get; set; }
        public decimal CurrentMonthAttendancePercentage { get; set; }
        public int SchoolDaysThisMonth { get; set; }
        public int StudentsWithAttendance { get; set; }
        public decimal TodayAttendancePercentage { get; set; }
        public int TodayTotalMarked { get; set; }
        
        // Fee Statistics
        public decimal TotalFeeCollected { get; set; }
        public decimal OverallFeeCollectionPercentage { get; set; }
        public long StudentsWithFeeRecords { get; set; }
        public decimal CurrentMonthFeeCollected { get; set; }
        public long CurrentMonthStudentsPaid { get; set; }
        public long CurrentMonthFeeTransactions { get; set; }
        public decimal TotalOutstandingFees { get; set; }
        public long StudentsWithOutstandingFees { get; set; }
    }

    public class AttendanceTrend
    {
        public DateTime Date { get; set; }
        public decimal AttendancePercentage { get; set; }
        public int TotalMarked { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
    }

    public class ClassAttendanceSummary
    {
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public Guid ClassId { get; set; }
        public int TotalAttendanceRecords { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int StudentsWithRecords { get; set; }
        public int DaysRecorded { get; set; }
        public decimal ClassAttendancePercentage { get; set; }
    }

    public class AttendanceSummaryResult
    {
        public long TotalSchoolDays { get; set; }
        public decimal AverageAttendancePercentage { get; set; }
        public long TotalAttendanceRecords { get; set; }
        public long UniqueStudentsCount { get; set; }
    }
}