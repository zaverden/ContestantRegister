using AutoMapper;
using ContestantRegister.Models;
using ContestantRegister.Models.AccountViewModels;
using ContestantRegister.ViewModels;

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

        }
    }
}
