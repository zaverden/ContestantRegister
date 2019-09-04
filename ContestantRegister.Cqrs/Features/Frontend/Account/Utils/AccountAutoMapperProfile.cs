using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels;
using ContestantRegister.Models;
using StudyPlaceDropdownItemViewModel = ContestantRegister.Cqrs.Features.Admin.Users.ViewModels.StudyPlaceDropdownItemViewModel;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.Utils
{
    public class AccountAutoMapperProfile : Profile
    {
        public AccountAutoMapperProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>();

            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => sp is School ? sp.ShortName : $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<Institution, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => $"{sp.ShortName} ({sp.FullName})"));
        }
    }
}

