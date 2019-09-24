using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.CompClasses.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Admin.CompClasses.QueryHandlers
{
    
    public class GetAreasForCompClassQueryHandler : ReadRepositoryQueryHandler<GetAreasForCompClassQuery, List<Area>>
    {
        public GetAreasForCompClassQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<List<Area>> HandleAsync(GetAreasForCompClassQuery query)
        {
            return await ReadRepository.Set<Area>().OrderBy(x => x.Name).ToListAsync();
        }
    }
}
