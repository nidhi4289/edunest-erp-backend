using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Api.Models
{
    public class CommunicationProfile : Profile
    {
        public CommunicationProfile()
        {
            CreateMap<CommunicationCreateDto, Communication>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

            CreateMap<CommunicationUpdateDto, Communication>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            CreateMap<Communication, CommunicationDto>();
            CreateMap<CommunicationDto, Communication>();
        }
    }
}