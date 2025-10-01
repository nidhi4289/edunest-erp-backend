using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Repositories;
using EduNestERP.Persistence.Entities;

public class DashboardProfile : Profile
{
    public DashboardProfile()
    {
        // Attendance mappings
        CreateMap<DashboardSummary, DashboardSummaryDto>();
        CreateMap<AttendanceTrend, AttendanceTrendDto>();
        CreateMap<ClassAttendanceSummary, ClassAttendanceDto>();
        CreateMap<AttendanceSummaryResult, AttendanceSummaryDto>();
        
        // Fee statistics mappings
        CreateMap<FeeStatistics, FeeStatisticsDto>();
        CreateMap<FeeCollectionTrend, FeeCollectionTrendDto>();
        CreateMap<ClassFeeSummary, ClassFeeSummaryDto>();
        CreateMap<OutstandingFeesSummary, OutstandingFeesDto>();
        CreateMap<FeeCollectionSummaryResult, FeeCollectionSummaryDto>();
    }
}