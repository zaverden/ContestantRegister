using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Domain;

namespace ContestantRegister.Controllers._Common.QueryHandlers
{
    public class GetEntityForDeleteQueryHandler<TEntity> : ReadRepositoryQueryHandler<GetEntityByIdForDeleteQuery<TEntity>, TEntity> where TEntity : class
    {
        public GetEntityForDeleteQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<TEntity> HandleAsync(GetEntityByIdForDeleteQuery<TEntity> query)
        {
            var res = await ReadRepository.FindAsync<TEntity>(query.Id);
            return res;
        }
    }
}
