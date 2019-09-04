using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Utils
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
                .ForMember(vm => vm.CityId, opt => opt.MapFrom(u => u.StudyPlace.CityId));

            CreateMap<EditUserViewModel, ApplicationUser>();

            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => sp is School ? sp.ShortName : $"{sp.ShortName} ({sp.FullName})"));

            CreateMap<Institution, StudyPlaceDropdownItemViewModel>()
                .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name))
                .ForMember(splivm => splivm.ShortName, opt => opt.MapFrom(sp => $"{sp.ShortName} ({sp.FullName})"));

        }
    }
}

