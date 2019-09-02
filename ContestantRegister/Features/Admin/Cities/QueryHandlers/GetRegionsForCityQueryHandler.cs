using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Cities.Queries;
using ContestantRegister.Domain;

namespace ContestantRegister.Controllers.Cities
{
    

    public class GetRegionsForCityQueryHandler : ContextQueryHandler<GetRegionsForCityQuery, List<Region>>
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
