using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels
{
    public class RegisterContestParticipantViewModel : UserViewModelBase
    {
        public int ContestId { get; set; }
    }
}
