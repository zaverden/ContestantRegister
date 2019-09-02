using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.CompClasses.Queries;
using ContestantRegister.Data;
using ContestantRegister.Domain;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Controllers.CompClasses.QueryHandlers
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
