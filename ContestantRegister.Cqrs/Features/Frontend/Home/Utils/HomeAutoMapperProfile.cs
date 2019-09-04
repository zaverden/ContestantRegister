using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Utils
{
    public class HomeAutoMapperProfile : Profile
    {
        public HomeAutoMapperProfile()
        {
            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => sp is School ? sp.ShortName : $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<Institution, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => $"{sp.ShortName} ({sp.FullName})"));
        }
    }
}

