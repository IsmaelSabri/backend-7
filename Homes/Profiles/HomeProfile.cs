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
            CreateMap<HomeDto, Home>();
            CreateMap<HomeDto, Flat>();
            CreateMap<HomeDto, House>();
            CreateMap<HomeDto, Room>();
            CreateMap<HomeDto, HolidayRent>();
            CreateMap<HomeDto, NewProject>();
            CreateMap<HomeDto, Home4rent>();
        }
    }
}