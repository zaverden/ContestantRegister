using System;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features._Common.QueryHandlers
{
    internal class GetEntityByIdForDeleteQueryHandler<TEntity, TKey> : ReadRepositoryQueryHandler<GetEntityByIdForDeleteQuery<TEntity, TKey>, TEntity> 
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
    {
        public GetEntityByIdForDeleteQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<TEntity> HandleAsync(GetEntityByIdForDeleteQuery<TEntity, TKey> query)
        {
            if (query.IncludeProperties == null)
                return await ReadRepository.FindAsync<TEntity>(query.Id);

            var items = ReadRepository.Set<TEntity>();
            foreach (var property in query.IncludeProperties)
            {
                items = items.Include(property);
            }

            return await items.SingleOrDefaultAsync(x => x.Id.Equals(query.Id));
        }
    }
}
