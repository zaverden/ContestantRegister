using System;
using System.Threading.Tasks;

namespace ContestantRegister.Infrastructure.Cqrs
{
    public abstract class HandlerDispatcher : IHandlerDispatcher
    {
        public Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            return GetService<ICommandHandler<TCommand>>().HandleAsync(command);
        }

        public Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            return HandleAsync<TResult>(typeof(IQueryHandler<,>), query);
        }

        private Task<TResult> HandleAsync<TResult>(Type handlerGenericType, object context)
        {
            var handlerType = handlerGenericType.MakeGenericType(context.GetType(), typeof(TResult));
            var handler = GetService(handlerType);
            var method = handler.GetType().GetMethod("HandleAsync");
            var res = method.Invoke(handler, new [] { context });
            var ret = (Task<TResult>)res;
            return ret;
        }

        protected abstract T GetService<T>();
        protected abstract object GetService(Type serviceType);
    }
}
