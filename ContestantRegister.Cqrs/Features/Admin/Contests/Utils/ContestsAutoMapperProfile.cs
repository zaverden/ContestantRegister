using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Models;
using ContestantRegister.Utils.ViewModelsSorting;
using ContestantRegister.ViewModels.ManageViewModels;

namespace ContestantRegister.Controllers.Account.Utils
{
    public class ContestsAutoMapperProfile : Profile
    {
        public ContestsAutoMapperProfile()
        {
            CreateMap<Contest, ContestListItemViewModel>();

            
            CreateMap<Contest, ContestDetailsViewModel>();
            CreateMap<ContestDetailsViewModel, Contest>()
                .ForMember(contest => contest.ContestAreas, opt => opt.Ignore());

        }
    }
}

