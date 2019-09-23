using ContestantRegister.Framework.Cqrs;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries
{
    public class GetExportedTeamContestParticipantsQuery : IQuery<ExcelPackage>
    {
        public int ContestId { get; set; }
    }
}
