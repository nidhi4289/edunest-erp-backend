using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Application.Mapper
{
    public class ClassProfile : Profile
    {
        public ClassProfile()
        {
            CreateMap<ClassDto, Class>()
            .ReverseMap();
            CreateMap<CreateClassDto, Class>()
            .ReverseMap();
        }

    }
    public class AssessmentProfile : Profile
    {
        public AssessmentProfile()
        {
            CreateMap<AssessmentDto, Assessment>()
            .ReverseMap();

            CreateMap<CreateAssessmentDto, Assessment>()
            .ReverseMap();

            CreateMap<UpdateAssessmentDto, Assessment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ReverseMap();
        }
    }
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<SubjectDto, Subject>()
            .ReverseMap();
        }
    }

    public class FeeAdminProfile : Profile
    {
        public FeeAdminProfile()
        {
            // CreateFeeAdminDto to FeeAdmin
            CreateMap<CreateFeeAdminDto, FeeAdmin>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // UpdateFeeAdminDto to FeeAdmin
            CreateMap<UpdateFeeAdminDto, FeeAdmin>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // FeeAdmin to FeeAdminDto
            CreateMap<FeeAdmin, FeeAdminDto>();
        }
    }
}
