using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Commands
{
    public class EditIndividualContestRegistrationCommand : ICommand
    {
        public int RegistrationId { get; set; }
        public EditIndividualContestRegistrationViewModel ViewModel { get; set; }
    }
}
