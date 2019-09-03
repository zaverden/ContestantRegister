using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.ViewModels.UserViewModels;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.Queries
{
    public class GetAdminsQuery : IQuery<List<UserAdminViewModel>>
    {
    }
}
