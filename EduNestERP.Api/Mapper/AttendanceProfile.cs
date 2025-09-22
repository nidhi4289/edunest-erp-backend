using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Mapper
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<AttendanceDto, Attendance>()
            .ReverseMap();
    
        }
    }
}
