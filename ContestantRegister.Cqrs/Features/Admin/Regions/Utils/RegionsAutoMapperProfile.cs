using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Regions.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Regions.Utils
{
    public class RegionsAutoMapperProfile : Profile
    {
        public RegionsAutoMapperProfile()
        {
            CreateMap<Region, RegionViewModel>()
                .ReverseMap();

            CreateMap<Region, Region>();
        }
    }
}
