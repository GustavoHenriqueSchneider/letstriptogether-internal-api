using AutoMapper;
using WebApi.DTOs;
using WebApi.Models;


namespace WebApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, UserDto>().ReverseMap(); 
            CreateMap<CreateUserDto, User>(); 
            CreateMap<UserPreference, UserPreferenceDto>().ReverseMap();           
            CreateMap<CreateUserDto, User>(MemberList.None);
        }
    }
}
