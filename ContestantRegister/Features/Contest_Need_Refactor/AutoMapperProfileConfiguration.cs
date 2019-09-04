using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.Contest;
using ContestantRegister.ViewModels.Contest.Registration;
using ContestantRegister.ViewModels.ListItem;
using StudyPlaceDropdownItemViewModel = ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels.StudyPlaceDropdownItemViewModel;

namespace ContestantRegister.Utils
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<ContestRegistrationViewModel, IndividualContestRegistration>()
                .ReverseMap();
            
            CreateMap<TeamContestRegistrationViewModel, TeamContestRegistration>()
                .ReverseMap();

            CreateMap<RegisterContestParticipantViewModel, ApplicationUser>();
            
            CreateMap<Contest, SortingViewModel>();

            CreateMap<CompClass, CompClassListItemViewModel>()
                .ForMember(vm => vm.Text, opt => opt.MapFrom(ca => ca.Name))
                .ForMember(vm => vm.Value, opt => opt.MapFrom(ca => ca.Id));

            CreateMap<ContestArea, ContestAreaListItemViewModel>()
                .ForMember(vm => vm.Text, opt => opt.MapFrom(ca => ca.Area.Name))
                .ForMember(vm => vm.Value, opt => opt.MapFrom(ca => ca.Id));

            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => sp is School ? sp.ShortName : $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<Institution, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<ApplicationUser, ViewModels.ListItemViewModels.UserListItemViewModel>()
                .ForMember(ulivm => ulivm.DisplayName, opt => opt.MapFrom(au => $"{au.Name} {au.Surname} ({au.Email})"));

            CreateMap<ContestRegistrationDTO, IndividualContestRegistration>()
                .ForMember(e => e.Status, opt => opt.Ignore())
                .ForMember(e => e.Number, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ContestRegistrationDTO, TeamContestRegistration>()
                .ForMember(e => e.Status, opt => opt.Ignore())
                .ForMember(e => e.Number, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
