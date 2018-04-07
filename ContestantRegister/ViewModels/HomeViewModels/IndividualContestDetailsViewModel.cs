using System.Collections.Generic;
using ContestantRegister.Models;

namespace ContestantRegister.ViewModels.HomeViewModels
{
    public class IndividualContestDetailsViewModel
    {
        public Contest Contest { get; set; }

        public ICollection<ContestRegistration> UserRegistrations { get; set; }

        public ContestRegistration ParticipantRegistration { get; set; }
    }
}
