using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.CompClasses.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.CompClasses.Utils
{
    public class CompClassesAutoMapperProfile : Profile
    {
        public CompClassesAutoMapperProfile()
        {
            //Details
            CreateMap<CompClass, CompClass>();
            
            //List
            CreateMap<CompClass, CompClassListItemViewModel>()
                .ForMember(x => x.Area, opt => opt.MapFrom(y => y.Area.Name));
        }
    }
}
