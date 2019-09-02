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

namespace ContestantRegister.Controllers.Schools
{
    public class GetSchoolForDeleteQueryHandler : GetEntityForDeleteQueryHandler<School>
    {
        public GetSchoolForDeleteQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<School> HandleAsync(GetEntityByIdForDeleteQuery<School> query)
        {
            var res = await ReadRepository.Set<School>()
                .Include(x => x.City)
                .SingleOrDefaultAsync(x => x.Id == query.Id);               

            return res;
        }
    }
}
