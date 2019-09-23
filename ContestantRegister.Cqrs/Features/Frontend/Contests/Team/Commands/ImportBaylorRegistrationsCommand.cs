using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands
{
    public class ImportBaylorRegistrationsCommand : ICommand
    {
        public ImportParticipantsViewModel ViewModel { get; set; }
    }
}
