using System.Collections.Generic;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels
{
    public class IndividualContestInfoViewModel : ContestInfoViewModel<IndividualContestRegistration>
    {
    }

    public class TeamContestInfoViewModel : ContestInfoViewModel<TeamContestRegistration>
    {
        public ApplicationUser User { get; set; }
    }

    public abstract class ContestInfoViewModel<TContestRegistration> : ContestInfoViewModelBase
        where TContestRegistration : ContestRegistration
    {
       

        public ICollection<TContestRegistration> ContestRegistrations { get; set; }

        public ICollection<TContestRegistration> UserRegistrations { get; set; }

        public TContestRegistration ParticipantRegistration { get; set; }
    }

    public abstract class ContestInfoViewModelBase
    {
        public Models.Contest Contest { get; set; }
    }
}
