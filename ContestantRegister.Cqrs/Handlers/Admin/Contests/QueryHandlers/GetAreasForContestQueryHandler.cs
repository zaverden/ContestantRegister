using System.Collections.Generic;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Contests.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

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
