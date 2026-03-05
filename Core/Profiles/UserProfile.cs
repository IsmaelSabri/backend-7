using AutoMapper;
using Core.Dto;
using Core.Models;

namespace Core.Profiles
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