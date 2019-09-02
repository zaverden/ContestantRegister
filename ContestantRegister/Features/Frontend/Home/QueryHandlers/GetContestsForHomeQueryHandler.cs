using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Data;
using ContestantRegister.Domain;
using ContestantRegister.Features.Frontend.Home.Queries;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Features.Frontend.Home.QueryHandlers
{
    public class GetContestsForHomeQueryHandler : ContextQueryHandler<GetContestsForHomeQuery, List<Contest>>
    {
        public GetContestsForHomeQueryHandler(IReadRepository repository) : base(repository)
        {

        }
        public override async Task<List<Contest>> HandleAsync(GetContestsForHomeQuery query)
        {
            var contests = query.IsArchived
                ? ReadRepository.Set<Contest>().Where(Contest.Archived)
                : ReadRepository.Set<Contest>().Where(!Contest.Archived);

            var result = query.IsOrderByDesc ?
                contests.OrderByDescending(x => x.Id) :
                contests.OrderBy(x => x.Id);
                
            return await result.ToListAsync(); ;
        }
    }
}
