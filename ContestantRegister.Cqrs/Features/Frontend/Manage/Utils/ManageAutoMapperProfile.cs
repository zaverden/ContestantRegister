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

            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>();
        }
    }
}

