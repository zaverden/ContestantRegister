using System;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Models;

namespace ContestantRegister.Controllers._Common.QueryHandlers
{
    
    public class GetEntityQueryHandler<TEntity, TKey> : ReadRepositoryQueryHandler<GetEntityByIdQuery<TEntity, TKey>, TEntity> 
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
    {
        public GetEntityQueryHandler(IReadRepository repository) : base(repository)
        {            
        }

        public override async Task<TEntity> HandleAsync(GetEntityByIdQuery<TEntity, TKey> query)
        {
            if (query.IncludeProperties == null)
                return await ReadRepository.FindAsync<TEntity>(query.Id);

            var items = ReadRepository.Set<TEntity>();
            foreach (var property in query.IncludeProperties)
            {
                items = items.Include(property);
            }

            return await items.SingleOrDefaultAsync(x => /*x.Id == query.Id*/ x.Id.Equals(query.Id));
        }
    }
}
