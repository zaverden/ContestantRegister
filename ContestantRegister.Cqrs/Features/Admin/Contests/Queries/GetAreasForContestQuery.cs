using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.Queries
{
    public class GetAreasForContestQuery : IQuery<List<Area>>

    {
    }
}
