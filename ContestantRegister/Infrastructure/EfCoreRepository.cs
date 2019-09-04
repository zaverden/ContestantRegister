using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Data;
using ContestantRegister.Domain.Repository;

namespace ContestantRegister.Infrastructure
{
    public class EfCoreRepository : IRepository
    {
        private readonly ApplicationDbContext _context;

        public EfCoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add<TEntity>(TEntity entity) //where TEntity : class
        {
            _context.Add(entity);
        }

        public Task<TEntity> FindAsync<TEntity>(object key) where TEntity : class
        {
            return _context.FindAsync<TEntity>(key);
        }


        public void Remove<TEntity>(TEntity entity)
        {
            _context.Remove(entity);
        }

        public void Update<TEntity>(TEntity entity)
        {
            _context.Update(entity);
        }

        public IQueryable<TEntity> Set<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
