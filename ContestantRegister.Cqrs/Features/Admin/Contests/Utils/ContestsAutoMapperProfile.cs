using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.Utils
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

