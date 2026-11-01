using AutoMapper;
using Core.Models;

namespace Core.Profiles
{
    public class HomeProfile : Profile
    {
        public HomeProfile()
        {
            CreateMap<Home, Home>().ValidateMemberList(MemberList.None);
            CreateMap<Home, Flat>().ValidateMemberList(MemberList.None);
            CreateMap<Home, House>().ValidateMemberList(MemberList.None);
            CreateMap<Home, Room>().ValidateMemberList(MemberList.None);
            CreateMap<Home, HolidayRent>().ValidateMemberList(MemberList.None);
            CreateMap<Home, NewProject>().ValidateMemberList(MemberList.None);
            CreateMap<Home, Other>().ValidateMemberList(MemberList.None);
        }
    }
}