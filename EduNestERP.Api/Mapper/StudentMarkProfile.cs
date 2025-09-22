using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Api.Mapper
{
    public class StudentMarkProfile : Profile
    {
        public StudentMarkProfile()
        {
            CreateMap<StudentMark, StudentMarkDto>().ReverseMap();
            CreateMap<StudentMarkDto, StudentMark>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}