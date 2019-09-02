using AutoMapper;
using ContestantRegister.Features.Frontend.Home.ViewModels;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.AccountViewModels;

namespace ContestantRegister.Controllers.Account.Utils
{
    public class HomeAutoMapperProfile : Profile
    {
        public HomeAutoMapperProfile()
        {
            CreateMap<StudyPlace, StudyPlaceDropdownItemViewModel>();
        }
    }
}

