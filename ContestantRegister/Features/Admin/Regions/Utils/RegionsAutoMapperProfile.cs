using AutoMapper;
using ContestantRegister.Controllers.Regions;
using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers
{
    public class RegionsAutoMapperProfile : Profile
    {
        public RegionsAutoMapperProfile()
        {
            CreateMap<Region, RegionViewModel>();
            CreateMap<RegionViewModel, Region>();
        }
    }
}
