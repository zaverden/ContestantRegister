using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Institutions.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Admin.Institutions.QueryHandlers
{
    internal class GetCitiesForInstitutionQueryHandler : ReadRepositoryQueryHandler<GetCitiesForInstitutionQuery, List<City>>
    {
        public GetCitiesForInstitutionQueryHandler(IReadRepository repository) : base(repository)
        {
        }
        public override async Task<List<City>> HandleAsync(GetCitiesForInstitutionQuery query)
        {
            var result = await ReadRepository.Set<City>()
                .OrderBy(item => item.Name)
                .ToListAsync();

            return result;
        }
    }
}
