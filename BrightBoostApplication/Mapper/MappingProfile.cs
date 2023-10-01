using AutoMapper;
using BrightBoostApplication.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using BrightBoostApplication.Models.ViewModel;

namespace BrightBoostApplication.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityRole, RoleModel>().ReverseMap();
            CreateMap<TermCourse, TermSubjectViewModel>()
            .ForMember(dest => dest.TermCourseId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.subjectName, opt => opt.MapFrom(src => src.Subject.subjectName))
            .ForMember(dest => dest.subjectDescription, opt => opt.MapFrom(src => src.Subject.subjectDescription))
            .ForMember(dest => dest.createdDate, opt => opt.MapFrom(src => src.Subject.createdDate))
            .ForMember(dest => dest.updateDate, opt => opt.MapFrom(src => src.Subject.updateDate))
            .ForMember(dest => dest.isActive, opt => opt.MapFrom(src => src.Subject.isActive))
            .ForMember(dest => dest.TermCourse, opt => opt.MapFrom(src => src.Subject.TermCourse))
            .ForMember(dest => dest.Expertise, opt => opt.MapFrom(src => src.Subject.Expertise));
        }
    }
}
