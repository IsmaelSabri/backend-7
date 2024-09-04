using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Homes.Dto;
using Homes.Models;

namespace Homes.Profiles
{
    public class HomeProfile : Profile
    {
        public HomeProfile()
        {
            CreateMap<HomeDto, Home>().ValidateMemberList(MemberList.None);
            CreateMap<HomeDto, Flat>().ValidateMemberList(MemberList.None);
            CreateMap<HomeDto, House>().ValidateMemberList(MemberList.None);
            CreateMap<HomeDto, Room>().ValidateMemberList(MemberList.None);
            CreateMap<HomeDto, HolidayRent>().ValidateMemberList(MemberList.None);
            CreateMap<HomeDto, NewProject>().ValidateMemberList(MemberList.None);/*.ForAllMembers(opts =>
            {
                opts.Condition((src, dest, srcMember) => srcMember != null);
            });*/
        }
    }
}