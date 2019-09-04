using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Cities.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Admin.Cities.QueryHandlers
{
    public class GetRegionsForCityQueryHandler : ReadRepositoryQueryHandler<GetRegionsForCityQuery, List<Region>>
    {
        public GetRegionsForCityQueryHandler(IReadRepository repository) : base(repository)
        {
        }
        public override async Task<List<Region>> HandleAsync(GetRegionsForCityQuery query)
        {
            var result = await ReadRepository.Set<Region>()
                .OrderBy(item => item.Name)
                .ToListAsync();

            return result;
        }
    }
}
