using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.UserViewModels;

namespace ContestantRegister.Controllers.Account.Utils
{
    public class UsersAutoMapperProfile : Profile
    {
        public UsersAutoMapperProfile()
        {
            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>();

            CreateMap<ApplicationUser, UserListItemViewModel>()
                .ForMember(x => x.StudyPlace, opt => opt.MapFrom(y => y.StudyPlace.ShortName))
                .ForMember(x => x.City, opt => opt.MapFrom(y => y.StudyPlace.City.Name));


            CreateMap<ApplicationUser, CreateUserViewModel>()
                .ReverseMap();

            CreateMap<ApplicationUser, EditUserViewModel>()
                .ReverseMap();

        }
    }
}

