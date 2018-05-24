using System;
using AutoMapper;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.AccountViewModels;
using ContestantRegister.ViewModels.ContestViewModels;
using ContestantRegister.ViewModels.HomeViewModels;
using ContestantRegister.ViewModels.ListItemViewModels;
using ContestantRegister.ViewModels.ManageViewModels;
using ContestantRegister.ViewModels.UserViewModels;
using ContestantRegister.ViewModels.Home;
using ContestantRegister.ViewModels.ListItem;

namespace ContestantRegister.Utils
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<City, City>();
            CreateMap<School, School>();
            CreateMap<Institution, Institution>();

            CreateMap<RegisterViewModel, ApplicationUser>();
            
            CreateMap<IndividualContestRegistrationViewModel, IndividualContestRegistration>();
            CreateMap<IndividualContestRegistration, IndividualContestRegistrationViewModel>();

            CreateMap<Contest, ContestViewModel>();
            CreateMap<ContestViewModel, Contest>();

            CreateMap<ApplicationUser, CreateUserViewModel>();
            CreateMap<CreateUserViewModel, ApplicationUser>();
            

            CreateMap<ApplicationUser, EditUserViewModel>();
            CreateMap<EditUserViewModel, ApplicationUser>();
            
            CreateMap<RegisterContestParticipantViewModel, ApplicationUser>();
            
            CreateMap<IndexViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, IndexViewModel>();

            CreateMap<Contest, SortingViewModel>();
            CreateMap<CompClass, CompClassListItemViewModel>();

            CreateMap<StudyPlace, StudyPlaceListItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => sp is School ? sp.ShortName : $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<Institution, StudyPlaceListItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<ApplicationUser, UserListItemViewModel>()
                .ForMember(ulivm => ulivm.DisplayName, opt => opt.MapFrom(au => $"{au.Name} {au.Surname} ({au.Email})"));

            CreateMap<IndividualContestRegistration, IndividualRegistrationDTO>()
                .ForMember(e => e.Email, opt => opt.MapFrom(r => r.Participant1.Email))
                .ForMember(e => e.Surname, opt => opt.MapFrom(r => r.Participant1.Surname))
                .ForMember(e => e.Name, opt => opt.MapFrom(r => r.Participant1.Name))
                .ForMember(e => e.Patronymic, opt => opt.MapFrom(r => r.Participant1.Patronymic))
                .ForMember(e => e.TrainerName, opt => opt.MapFrom(r => $"{r.Trainer.Surname} {r.Trainer.Name} {r.Trainer.Patronymic}"))
                .ForMember(e => e.TrainerEmail, opt => opt.MapFrom(r => r.Trainer.Email))
                .ForMember(e => e.ManagerName, opt => opt.MapFrom(r => $"{r.Manager.Surname} {r.Manager.Name} {r.Manager.Patronymic}"))
                .ForMember(e => e.ManagerEmail, opt => opt.MapFrom(r => r.Manager.Email))
                .ForMember(e => e.Region, opt => opt.MapFrom(r => r.StudyPlace.City.Region.Name))
                .ForMember(e => e.City, opt => opt.MapFrom(r => r.StudyPlace.City.Name))
                .ForMember(e => e.StudyPlace, opt => opt.MapFrom(r => r.StudyPlace.ShortName));

            CreateMap<IndividualRegistrationDTO, IndividualContestRegistration>()
                .ForMember(e => e.Status, opt => opt.Ignore())
                .ForMember(e => e.Number, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
