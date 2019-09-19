using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    public abstract class QueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected readonly IQueryHandler<TQuery, TResult> Next;

        protected QueryHandlerDecorator(IQueryHandler<TQuery, TResult> next)
        {
            Next = next;
        }

        public abstract Task<TResult> HandleAsync(TQuery query);
    }
}
