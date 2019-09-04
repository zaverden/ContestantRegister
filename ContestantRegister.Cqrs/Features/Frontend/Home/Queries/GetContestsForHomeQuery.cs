using System.Collections.Generic;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Queries
{
    public class GetContestsForHomeQuery : IQuery<List<Contest>>
    {
        public bool IsArchived { get; set; }

        public bool IsOrderByDesc { get; set; }
    }
}
