using AutoMapper;
using EduNestERP.Api.Models;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Mapper
{
    public class StudentFeeProfile : Profile
    {
        public StudentFeeProfile()
        {
            CreateMap<StudentFeeDto, StudentFee>()
            .ForMember(dest => dest.StudentEduNestId,
             opt => opt.MapFrom(src => string.IsNullOrEmpty(src.StudentEduNestId) ? $"ST-{src.FirstName}{src.LastName}{src.DateOfBirth:yyyyMMdd}" : src.StudentEduNestId))
            .ReverseMap();
        }
    }
}
