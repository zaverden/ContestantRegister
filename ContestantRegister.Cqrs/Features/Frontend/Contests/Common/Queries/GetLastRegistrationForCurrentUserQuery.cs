using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Queries
{
    public class GetLastRegistrationForCurrentUserQuery : IQuery<ContestRegistration>
    {
    }
}
