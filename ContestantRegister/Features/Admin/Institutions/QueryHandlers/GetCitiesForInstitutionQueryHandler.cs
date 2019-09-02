using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers.Institutions.Queries
{
    

    public class GetCitiesForInstitutionQueryHandler : ContextQueryHandler<CitiesForInstitutionQuery, List<City>>
    {
        public GetCitiesForInstitutionQueryHandler(IReadRepository repository) : base(repository)
        {
        }
        public override async Task<List<City>> HandleAsync(CitiesForInstitutionQuery query)
        {
            var result = await ReadRepository.Set<City>()
                .OrderBy(item => item.Name)
                .ToListAsync();

            return result;
        }
    }
}
