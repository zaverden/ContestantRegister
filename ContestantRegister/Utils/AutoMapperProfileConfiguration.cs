﻿using AutoMapper;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.AccountViewModels;
using ContestantRegister.ViewModels.HomeViewModels;
using ContestantRegister.ViewModels.ListItemViewModels;
using ContestantRegister.ViewModels.ManageViewModels;
using ContestantRegister.ViewModels.UserViewModels;

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

            CreateMap<ApplicationUser, CreateUserViewModel>();
            CreateMap<CreateUserViewModel, ApplicationUser>();
            

            CreateMap<ApplicationUser, EditUserViewModel>();
            CreateMap<EditUserViewModel, ApplicationUser>();
            
            CreateMap<RegisterContestParticipantViewModel, ApplicationUser>();
            
            CreateMap<IndexViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, IndexViewModel>();

            CreateMap<StudyPlace, StudyPlaceListItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name));

            CreateMap<ApplicationUser, UserListItemViewModel>()
                .ForMember(ulivm => ulivm.DisplayName, opt => opt.MapFrom(au => $"{au.Name} {au.Surname} ({au.Email})"));

        }
    }
}
