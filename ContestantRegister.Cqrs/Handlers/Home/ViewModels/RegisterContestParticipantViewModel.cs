using ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels
{
    public class RegisterContestParticipantViewModel : UserViewModelBase
    {
        public int ContestId { get; set; }
    }
}
