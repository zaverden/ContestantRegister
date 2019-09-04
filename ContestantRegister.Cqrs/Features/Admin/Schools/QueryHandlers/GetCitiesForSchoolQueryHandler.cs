using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Schools.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Admin.Schools.QueryHandlers
{
    
    public class GetCitiesForSchoolQueryHandler : ReadRepositoryQueryHandler<CitiesForSchoolQuery, List<City>>
    {
        public GetCitiesForSchoolQueryHandler(IReadRepository repository) : base(repository)
        {        
        }
        public override async Task<List<City>> HandleAsync(CitiesForSchoolQuery query)
        {
            var result = await ReadRepository.Set<City>()
                .OrderBy(item => item.Name)
                .ToListAsync();

            return result;
        }
    }
}
