using System.Threading.Tasks;

namespace ContestantRegister.Infrastructure.Cqrs
{
    //TODO можно добавить CancellationToken для асинхронных методов Handle, будет архитектурненько, но не нужно :)
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
