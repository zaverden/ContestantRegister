using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Models;

namespace ContestantRegister.Controllers._Common.QueryHandlers
{
    
    public class GetEntityQueryHandler<TEntity> : ReadRepositoryQueryHandler<GetEntityByIdQuery<TEntity>, TEntity> where TEntity : DomainObject
    {
        public GetEntityQueryHandler(IReadRepository repository) : base(repository)
        {            
        }

        public override async Task<TEntity> HandleAsync(GetEntityByIdQuery<TEntity> query)
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
