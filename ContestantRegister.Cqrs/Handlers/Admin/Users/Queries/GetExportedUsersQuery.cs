using ContestantRegister.Framework.Cqrs;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Queries
{
    public class GetExportedUsersQuery : IQuery<ExcelPackage>
    {
    }
}
