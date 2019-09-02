using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Schools.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers.Schools
{
    
    public class GetCitiesForSchoolQueryHandler : ContextQueryHandler<CitiesForSchoolQuery, List<City>>
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
