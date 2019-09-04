using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Cities.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Cities.Utils
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
