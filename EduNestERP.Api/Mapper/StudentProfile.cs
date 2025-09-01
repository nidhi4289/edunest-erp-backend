using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;


public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentDto>().ReverseMap();
    }
}