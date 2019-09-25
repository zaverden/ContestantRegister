using System.Collections.Generic;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Queries
{
    public class GetAdminsQuery : IQuery<List<UserAdminViewModel>>
    {
    }
}
