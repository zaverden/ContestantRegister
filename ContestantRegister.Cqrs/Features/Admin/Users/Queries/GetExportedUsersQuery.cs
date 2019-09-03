using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.Commands
{
    public class GetExportedUsersQuery : IQuery<ExcelPackage>
    {
    }
}
