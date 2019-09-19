using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Data;
using ContestantRegister.Domain.Repository;

namespace ContestantRegister.Infrastructure
{
    public class EfCoreRepository : EfCoreReadRepository, IRepository
    {
        public EfCoreRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void Add<TEntity>(TEntity entity) 
        {
            Context.Add(entity);
        }

        public void Remove<TEntity>(TEntity entity)
        {
            Context.Remove(entity);
        }

        public void Update<TEntity>(TEntity entity)
        {
            Context.Update(entity);
        }

        public override IQueryable<TEntity> Set<TEntity>() 
        {
            //включаем changetracking при выборке 
            return Context.Set<TEntity>();
        }

        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
