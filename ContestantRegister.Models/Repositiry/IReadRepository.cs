using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Domain
{
    public interface IReadRepository : IDisposable
    {
        Task<TEntity> FindAsync<TEntity>(object key) where TEntity : class;

        IQueryable<TEntity> Set<TEntity>() where TEntity : class;
    }
}
