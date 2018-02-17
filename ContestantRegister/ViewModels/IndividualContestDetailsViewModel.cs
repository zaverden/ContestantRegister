using System.Collections.Generic;
using ContestantRegister.Models;

namespace ContestantRegister.ViewModels
{
    public class IndividualContestDetailsViewModel
    {
        public Contest Contest { get; set; }

        public ICollection<ContestRegistration> UseRegistrations { get; set; }
    }
}
