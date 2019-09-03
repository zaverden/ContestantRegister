using System.Collections.Generic;
using ContestantRegister.Models;

namespace ContestantRegister.ViewModels.Contest
{
    public class IndividualContestDetailsViewModel : ContestDetailsViewModel<IndividualContestRegistration>
    {
    }

    public class TeamContestDetailsViewModel : ContestDetailsViewModel<TeamContestRegistration>
    {
        public ApplicationUser User { get; set; }
    }

    public abstract class ContestDetailsViewModel<TContestRegistration>
        where TContestRegistration : ContestRegistration
    {
        public Models.Contest Contest { get; set; }

        public ICollection<TContestRegistration> ContestRegistrations { get; set; }

        public ICollection<TContestRegistration> UserRegistrations { get; set; }

        public TContestRegistration ParticipantRegistration { get; set; }
    }
}
