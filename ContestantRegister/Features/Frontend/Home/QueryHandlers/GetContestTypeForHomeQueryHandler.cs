using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Data;
using ContestantRegister.Domain;
using ContestantRegister.Features.Frontend.Home.Queries;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Features.Frontend.Home.QueryHandlers
{
    public class GetContestTypeForHomeQueryHandler : ContextQueryHandler<GetContestTypeForHomeQuery, ContestType>
    {
        public GetContestTypeForHomeQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<ContestType> HandleAsync(GetContestTypeForHomeQuery query)
        {
            return await ReadRepository.Set<Contest>()
                            .Where(x => x.Id == query.Id)
                            .Select(x => x.ContestType)
                            .SingleOrDefaultAsync();
        }
    }
}
