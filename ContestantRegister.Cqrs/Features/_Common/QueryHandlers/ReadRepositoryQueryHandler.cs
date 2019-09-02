using System.Threading.Tasks;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common.QueryHandlers
{
    public abstract class ReadRepositoryQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected readonly IReadRepository ReadRepository;

        protected ReadRepositoryQueryHandler(IReadRepository repository)
        {
            ReadRepository = repository;
        }

        public abstract Task<TResult> HandleAsync(TQuery query);
    }
}
