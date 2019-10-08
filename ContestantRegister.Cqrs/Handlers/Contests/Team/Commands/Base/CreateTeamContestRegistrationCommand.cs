using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands
{

    public abstract class CreateTeamContestRegistrationCommand : ICommand
    {
        public CreateTeamContestRegistrationViewModel ViewModel { get; set; }
        
    }
}
