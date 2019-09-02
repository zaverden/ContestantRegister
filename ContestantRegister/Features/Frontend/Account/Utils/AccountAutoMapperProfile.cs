using AutoMapper;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.AccountViewModels;

namespace ContestantRegister.Controllers.Account.Utils
{
    public class AccountAutoMapperProfile : Profile
    {
        public AccountAutoMapperProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>();
        }
    }
}

