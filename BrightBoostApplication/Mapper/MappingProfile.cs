using AutoMapper;
using BrightBoostApplication.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace BrightBoostApplication.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityRole, RoleModel>().ReverseMap();
        }
    }
}
