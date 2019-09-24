using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands
{
    public class EditTeamContestRegistrationCommand : ICommand
    {
        public EditTeamContestRegistrationViewModel ViewModel { get; set; }
        public int RegistrationId { get; set; }
    }
}
