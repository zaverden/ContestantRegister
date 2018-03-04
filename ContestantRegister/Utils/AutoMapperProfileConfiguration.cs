using AutoMapper;
using ContestantRegister.Models;
using ContestantRegister.ViewModels;
using ContestantRegister.ViewModels.AccountViewModels;
using ContestantRegister.ViewModels.HomeViewModels;
using ContestantRegister.ViewModels.ManageViewModels;
using ContestantRegister.ViewModels.UserViewModels;

namespace ContestantRegister
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<City, City>();
            CreateMap<School, School>();
            CreateMap<Institution, Institution>();

            CreateMap<RegisterViewModel, Pupil>();
            CreateMap<RegisterViewModel, Student>();
            CreateMap<RegisterViewModel, Trainer>();

            CreateMap<IndividualContestRegistrationViewModel, IndividualContestRegistration>();
            CreateMap<IndividualContestRegistration, IndividualContestRegistrationViewModel>();

            CreateMap<ContestantUser, CreateUserViewModel>();
            CreateMap<CreateUserViewModel, ContestantUser>();
            CreateMap<Student, CreateUserViewModel>();
            CreateMap<CreateUserViewModel, Student>();
            CreateMap<Pupil, CreateUserViewModel>();
            CreateMap<CreateUserViewModel, Pupil>();
            CreateMap<Trainer, CreateUserViewModel>();
            CreateMap<CreateUserViewModel, Trainer>();

            CreateMap<ContestantUser, EditUserViewModel>();
            CreateMap<EditUserViewModel, ContestantUser>();
            CreateMap<Student, EditUserViewModel>();
            CreateMap<EditUserViewModel, Student>();
            CreateMap<Pupil, EditUserViewModel>();
            CreateMap<EditUserViewModel, Pupil>();
            CreateMap<Trainer, EditUserViewModel>();
            CreateMap<EditUserViewModel, Trainer>();

            CreateMap<RegisterContestParticipantViewModel, Trainer>();
            CreateMap<RegisterContestParticipantViewModel, Student>();
            CreateMap<RegisterContestParticipantViewModel, Pupil>();

            CreateMap<IndexViewModel, ApplicationUser>();
            CreateMap<IndexViewModel, Pupil>();
            CreateMap<IndexViewModel, Student>();
            CreateMap<IndexViewModel, Trainer>();
            CreateMap<Pupil, IndexViewModel>();
            CreateMap<Student, IndexViewModel>();
            CreateMap<Trainer, IndexViewModel>();
            CreateMap<ApplicationUser, IndexViewModel>();

        }
    }
}
