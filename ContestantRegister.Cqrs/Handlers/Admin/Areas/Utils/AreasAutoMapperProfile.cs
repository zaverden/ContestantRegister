using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Areas.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Areas.Utils
{
    public class AreasAutoMapperProfile : Profile
    {
        public AreasAutoMapperProfile()
        {
            //List
            CreateMap<Area, AreaViewModel>()
                .ReverseMap();
            
            //Details
            CreateMap<Area, Area>();
        }
    }
}

