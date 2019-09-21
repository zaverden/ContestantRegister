using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Utils
{
    public class UsersAutoMapperProfile : Profile
    {
        public UsersAutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserListItemViewModel>()
                .ForMember(x => x.StudyPlace, opt => opt.MapFrom(y => y.StudyPlace.ShortName))
                .ForMember(x => x.City, opt => opt.MapFrom(y => y.StudyPlace.City.Name));


            CreateMap<ApplicationUser, CreateUserViewModel>()
                .ReverseMap();

            CreateMap<ApplicationUser, EditUserViewModel>()
                .ForMember(vm => vm.CityId, opt => opt.MapFrom(u => u.StudyPlace.CityId));

            CreateMap<EditUserViewModel, ApplicationUser>();
        }
    }
}

