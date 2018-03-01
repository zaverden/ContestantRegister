using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Models.AccountViewModels;

namespace ContestantRegister.ViewModels
{
    public class RegisterContestParticipantViewModel : UserViewModelBase
    {
        public int ContestId { get; set; }
    }
}
