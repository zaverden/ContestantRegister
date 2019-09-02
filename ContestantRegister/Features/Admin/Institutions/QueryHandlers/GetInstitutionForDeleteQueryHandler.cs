using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Domain;

namespace ContestantRegister.Controllers
{
    public class GetInstitutionForDeleteQueryHandler : GetEntityForDeleteQueryHandler<Institution>
    {
        public GetInstitutionForDeleteQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<Institution> HandleAsync(GetEntityByIdForDeleteQuery<Institution> query)
        {
            var res = await ReadRepository.Set<Institution>()
                .Include(x => x.City)
                .SingleOrDefaultAsync(x => x.Id == query.Id);               

            return res;
        }
    }
}
