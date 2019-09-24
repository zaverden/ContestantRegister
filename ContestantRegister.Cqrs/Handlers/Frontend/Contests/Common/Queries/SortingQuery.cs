using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Queries
{

    public class SortingQueryResult
    {
        public int[] CompClassIds { get; set; }
        public SortingViewModel ViewModel { get; set; }
    }

    public class SortingQuery : IQuery<SortingQueryResult>
    {
        public int ContestId { get; set; }
    }
}
