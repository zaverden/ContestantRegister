using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Domain;

namespace ContestantRegister.Controllers._Common.QueryHandlers
{
    
    public class GetEntityQueryHandler<TEntity> : ContextQueryHandler<GetEntityByIdQuery<TEntity>, TEntity> where TEntity : class
    {
        public GetEntityQueryHandler(IReadRepository repository) : base(repository)
        {            
        }

        public override async Task<TEntity> HandleAsync(GetEntityByIdQuery<TEntity> query)
        {
            var result = await ReadRepository.FindAsync<TEntity>(query.Id);          

            return result;
        }
    }
}
