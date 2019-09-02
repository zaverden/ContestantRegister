using AutoMapper;
using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.Cities
{
    public class CitiesAutoMapperProfile : Profile
    {
        public CitiesAutoMapperProfile()
        {
            CreateMap<City, City>();
            CreateMap<City, CityListItemViewModel>()
                .ForMember(x => x.Region, opt => opt.MapFrom(y => y.Region.Name));
        }
    }
}
