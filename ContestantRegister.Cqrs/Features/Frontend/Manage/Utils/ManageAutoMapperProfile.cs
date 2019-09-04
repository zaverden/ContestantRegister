using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.ManageViewModels;

namespace ContestantRegister.Controllers.Account.Utils
{
    public class ManageAutoMapperProfile : Profile
    {
        public ManageAutoMapperProfile()
        {
            CreateMap<IndexViewModel, ApplicationUser>()
                .ReverseMap();

            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => sp is School ? sp.ShortName : $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<Institution, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => $"{sp.ShortName} ({sp.FullName})"));
        }
    }
}

