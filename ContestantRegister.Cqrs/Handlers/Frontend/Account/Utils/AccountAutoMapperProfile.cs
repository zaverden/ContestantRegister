using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.Utils
{
    public class AccountAutoMapperProfile : Profile
    {
        public AccountAutoMapperProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>();
        }
    }
}

