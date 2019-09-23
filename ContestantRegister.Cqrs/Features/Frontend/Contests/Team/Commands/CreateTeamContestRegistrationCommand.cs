using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands
{

    public class CreateTeamContestRegistrationCommand : ICommand
    {
        public CreateTeamContestRegistrationViewModel ViewModel { get; set; }
        
    }
}
