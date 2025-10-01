namespace EduNestERP.Persistence.Entities
{
    public class FeeStatistics
    {
        public long TotalFeeRecords { get; set; }
        public decimal TotalFeeCollected { get; set; }
        public decimal TotalFeeWaived { get; set; }
        public decimal TotalFeeDue { get; set; }
        public long StudentsWithFeeRecords { get; set; }
        public decimal AverageFeeCollected { get; set; }
        public decimal FeeCollectionPercentage { get; set; }
    }

    public class CurrentMonthFeeCollection
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long TotalTransactions { get; set; }
        public decimal TotalCollectedThisMonth { get; set; }
        public decimal TotalWaivedThisMonth { get; set; }
        public long UniqueStudentsPaid { get; set; }
        public decimal AverageCollectionPerTransaction { get; set; }
        public decimal HighestCollection { get; set; }
        public decimal LowestCollection { get; set; }
    }

    public class ClassFeeSummary
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

    public class FeeAdminSummary
    {
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public decimal TotalAnnualFee { get; set; }
        public decimal MonthlyFee { get; set; }
        public bool IsActive { get; set; }
        public decimal ActualCollected { get; set; }
        public long StudentsPaid { get; set; }
        public long TotalStudents { get; set; }
        public decimal CollectionVsTargetPercentage { get; set; }
    }

    public class DailyFeeCollection
    {
        public DateTime CollectionDate { get; set; }
        public long TransactionsCount { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalWaived { get; set; }
        public long UniqueStudents { get; set; }
        public decimal AveragePerTransaction { get; set; }
    }

    public class FeeCollectionTrend
    {
        public DateTime CollectionDate { get; set; }
        public decimal TotalCollected { get; set; }
        public long TransactionsCount { get; set; }
        public long UniqueStudents { get; set; }
        public decimal AveragePerTransaction { get; set; }
    }

    public class OutstandingFeesSummary
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

    public class FeeCollectionSummaryResult
    {
        public long TotalCollectionDays { get; set; }
        public decimal TotalAmountCollected { get; set; }
        public long TotalTransactions { get; set; }
        public long UniqueStudentsPaid { get; set; }
        public decimal AverageDailyCollection { get; set; }
        public decimal AveragePerTransaction { get; set; }
    }
}