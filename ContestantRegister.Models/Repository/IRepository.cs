using System;
using System.Threading.Tasks;
using ContestantRegister.Models;

namespace ContestantRegister.Domain.Repository
{
    public interface IRepository : IReadRepository
    {
        void Add<TEntity>( /*[NotNull]*/ TEntity entity);// where TEntity : class;

        void Remove<TEntity>( /*[NotNull]*/ TEntity entity);

        void RemoveById<TEntity, TKey>(TKey id) 
            where TEntity : class, IHasId<TKey>, new()
            where TKey : IEquatable<TKey>;

        void Update<TEntity>(TEntity entity);

        Task SaveChangesAsync();
    }
}
