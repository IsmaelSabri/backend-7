using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto;
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