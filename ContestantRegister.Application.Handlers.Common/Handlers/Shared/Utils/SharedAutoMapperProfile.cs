using AutoMapper;
using ContestantRegister.Cqrs.Features.Shared.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Shared.Utils
{
    public class SharedAutoMapperProfile : Profile
    {
        public SharedAutoMapperProfile()
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

