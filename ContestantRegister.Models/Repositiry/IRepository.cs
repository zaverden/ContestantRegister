using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Domain
{
    public interface IRepository : IReadRepository
    {
        void Add<TEntity>( /*[NotNull]*/ TEntity entity);// where TEntity : class;

        void Remove<TEntity>( /*[NotNull]*/ TEntity entity);

        void Update<TEntity>(TEntity entity);

        Task SaveChangesAsync();
    }
}
