using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Models;

namespace ContestantRegister.Controllers._Common.QueryHandlers
{
    public class GetEntityForDeleteQueryHandler<TEntity> : ReadRepositoryQueryHandler<GetEntityByIdForDeleteQuery<TEntity>, TEntity> where TEntity : DomainObject
    {
        public GetEntityForDeleteQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<TEntity> HandleAsync(GetEntityByIdForDeleteQuery<TEntity> query)
        {
            if (query.IncludeProperties == null)
                return await ReadRepository.FindAsync<TEntity>(query.Id);

            var items = ReadRepository.Set<TEntity>();
            foreach (var property in query.IncludeProperties)
            {
                items = items.Include(property);
            }

            return await items.SingleOrDefaultAsync(x => x.Id == query.Id);
        }
    }
}
