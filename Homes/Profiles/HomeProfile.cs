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
            CreateMap<HomeDto,Home>();
        }
    }
}