using System;
using AutoMapper;
using EduNestERP.Api.Model;
using EduNestERP.Persistence.Entities;

namespace EduNestERP.Api.Mapper
{
    public class HomeworkProfile : Profile
    {
        public HomeworkProfile()
        {
            CreateMap<Homework, HomeworkDto>().ReverseMap();
            CreateMap<CreateHomeworkDto, Homework>();
            CreateMap<UpdateHomeworkDto, Homework>();
        }
    }
}
