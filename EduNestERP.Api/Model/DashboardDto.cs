namespace EduNestERP.Api.Model
{
    public class DashboardSummaryDto
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

    public class AttendanceTrendDto
    {
        public DateTime Date { get; set; }
        public decimal AttendancePercentage { get; set; }
        public int TotalMarked { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
    }

    public class ClassAttendanceDto
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

    public class AttendanceSummaryDto
    {
        public long TotalSchoolDays { get; set; }
        public decimal AverageAttendancePercentage { get; set; }
        public long TotalAttendanceRecords { get; set; }
        public long UniqueStudentsCount { get; set; }
    }

    // Fee Statistics DTOs
    public class FeeStatisticsDto
    {
        public long TotalFeeRecords { get; set; }
        public decimal TotalFeeCollected { get; set; }
        public decimal TotalFeeWaived { get; set; }
        public decimal TotalFeeDue { get; set; }
        public long StudentsWithFeeRecords { get; set; }
        public decimal AverageFeeCollected { get; set; }
        public decimal FeeCollectionPercentage { get; set; }
    }

    public class FeeCollectionTrendDto
    {
        public DateTime CollectionDate { get; set; }
        public decimal TotalCollected { get; set; }
        public long TransactionsCount { get; set; }
        public long UniqueStudents { get; set; }
        public decimal AveragePerTransaction { get; set; }
    }

    public class ClassFeeSummaryDto
    {
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public Guid ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public long TotalFeeTransactions { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalWaived { get; set; }
        public long StudentsPaid { get; set; }
        public long TotalStudentsInClass { get; set; }
        public decimal AverageFeePerStudent { get; set; }
        public decimal PaymentCompletionPercentage { get; set; }
    }

    public class OutstandingFeesDto
    {
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public long TotalStudents { get; set; }
        public long StudentsPaid { get; set; }
        public long StudentsNotPaid { get; set; }
        public decimal ExpectedFeePerStudent { get; set; }
        public decimal TotalExpectedCollection { get; set; }
        public decimal ActualCollection { get; set; }
        public decimal OutstandingAmount { get; set; }
    }

    public class FeeCollectionSummaryDto
    {
        public long TotalCollectionDays { get; set; }
        public decimal TotalAmountCollected { get; set; }
        public long TotalTransactions { get; set; }
        public long UniqueStudentsPaid { get; set; }
        public decimal AverageDailyCollection { get; set; }
        public decimal AveragePerTransaction { get; set; }
    }

    public class DashboardDto
    {
        public DashboardSummaryDto Summary { get; set; } = new();
        public List<AttendanceTrendDto> RecentTrends { get; set; } = new();
        public List<ClassAttendanceDto> ClassAttendance { get; set; } = new();
        public List<FeeCollectionTrendDto> FeeCollectionTrends { get; set; } = new();
        public List<ClassFeeSummaryDto> ClassFeesSummary { get; set; } = new();
        public List<OutstandingFeesDto> OutstandingFees { get; set; } = new();
    }
}