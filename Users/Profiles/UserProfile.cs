using AutoMapper;
using Users.Dto;
using Users.Models;

namespace Users.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<UserReadyDto, User>();
        }
    }
}