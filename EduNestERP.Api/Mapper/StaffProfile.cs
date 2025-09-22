using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;

public class StaffProfile : Profile
{
    public StaffProfile()
    {
        CreateMap<Staff, StaffDto>().ReverseMap();
    }
}