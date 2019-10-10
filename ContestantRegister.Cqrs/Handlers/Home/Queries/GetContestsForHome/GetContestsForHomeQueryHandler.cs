using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.QueryHandlers
{
    internal class GetContestsForHomeQueryHandler : ReadRepositoryQueryHandler<GetContestsForHomeQuery, List<Contest>>
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
