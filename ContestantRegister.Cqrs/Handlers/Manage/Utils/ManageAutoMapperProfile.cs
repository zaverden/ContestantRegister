using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Utils
{
    public class ManageAutoMapperProfile : Profile
    {
        public ManageAutoMapperProfile()
        {
            CreateMap<IndexViewModel, ApplicationUser>()
                .ReverseMap();
        }
    }
}

