using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels
{
    public class UserForContestRegistrationListItemViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public UserType UserType { get; set; }
    }
}
