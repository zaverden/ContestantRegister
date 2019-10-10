using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.QueryHandlers
{
    internal class GetContestTypeForHomeQueryHandler : ReadRepositoryQueryHandler<GetContestTypeForHomeQuery, ContestType>
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
