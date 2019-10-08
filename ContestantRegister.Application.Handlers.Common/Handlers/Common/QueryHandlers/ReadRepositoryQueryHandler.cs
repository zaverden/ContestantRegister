using System.Threading.Tasks;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features._Common.QueryHandlers
{
    internal abstract class ReadRepositoryQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected readonly IReadRepository ReadRepository;

        protected ReadRepositoryQueryHandler(IReadRepository repository)
        {
            ReadRepository = repository;
        }

        public abstract Task<TResult> HandleAsync(TQuery query);
    }
}
