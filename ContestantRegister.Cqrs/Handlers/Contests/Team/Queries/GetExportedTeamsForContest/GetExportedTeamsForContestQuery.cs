using ContestantRegister.Framework.Cqrs;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries
{
    public class ExportTeamsResult
    {
        public ExcelPackage ExcelPackage { get; set; }

        public string ContestName { get; set; }
    }

    public class GetExportedTeamsForContestQuery : IQuery<ExportTeamsResult>
    {
        public int ContestId { get; set; }
    }
}
