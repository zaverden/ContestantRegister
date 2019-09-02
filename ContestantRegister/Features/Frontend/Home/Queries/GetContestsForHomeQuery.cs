using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Features.Frontend.Home.Queries
{
    public class GetContestsForHomeQuery : IQuery<List<Contest>>
    {
        public bool IsArchived { get; set; }

        public bool IsOrderByDesc { get; set; }
    }
}
