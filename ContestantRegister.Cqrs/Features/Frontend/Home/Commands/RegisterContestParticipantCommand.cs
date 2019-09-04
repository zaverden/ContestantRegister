using ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Commands
{
    public class RegisterContestParticipantCommand : ICommand
    {
        public RegisterContestParticipantViewModel RegisterContestParticipantViewModel { get; set; }
        public string CurrentUserEmail { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Scheme { get; set; }
    }
}
