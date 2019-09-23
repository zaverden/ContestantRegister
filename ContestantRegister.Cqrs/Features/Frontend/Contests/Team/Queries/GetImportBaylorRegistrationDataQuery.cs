using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries
{
    public class GetImportBaylorRegistrationDataQuery : IQuery<ImportParticipantsViewModel>
    {
        public int ContestId { get; set; }
    }
}
