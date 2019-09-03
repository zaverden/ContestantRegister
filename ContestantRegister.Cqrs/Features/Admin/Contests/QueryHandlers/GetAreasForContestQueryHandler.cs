using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Contests.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Models;
using ContestantRegister.Utils.ViewModelsSorting;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.QueryHandlers
{
    public class GetAreasForContestQueryHandler : ReadRepositoryQueryHandler<GetAreasForContestQuery, List<Area>>

    {
        public GetAreasForContestQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<List<Area>> HandleAsync(GetAreasForContestQuery query)
        {
            return await ReadRepository.Set<Area>()
                .ToListAsync();
        }
    }
}
