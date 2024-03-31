using AutoMapper;
using WebApplication.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using WebApplication.Models.ViewModel;

namespace WebApplication.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityRole, RoleModel>().ReverseMap();
            CreateMap<RetailerVM, Retailer>().ReverseMap();
            CreateMap<BranchVM, Branch>().ReverseMap();
        }
    }
}
