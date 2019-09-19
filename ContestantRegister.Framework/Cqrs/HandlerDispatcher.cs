using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Framework.Cqrs
{
    public class HandlerDispatcher : IHandlerDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MiddlewareMetadata _middlewareMetadata;

        public HandlerDispatcher(IServiceProvider serviceProvider, MiddlewareMetadata middlewareMetadata)
        {
            _serviceProvider = serviceProvider;
            _middlewareMetadata = middlewareMetadata;
        }

        public Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            object cur = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            for (int i = _middlewareMetadata.CommandMiddlewares.Count - 1; i >=0; i--)
            {
                var middlewareType = _middlewareMetadata.CommandMiddlewares[i];
                //Todo рефакторинг? добавить вытягивание параметров из сервис-провайдера
                var ctor = middlewareType.GetConstructors().Single();
                cur = ctor.Invoke(new object[] {cur});
            }

            dynamic d = cur;
            object res = d.HandleAsync(command);
            return (Task) res;
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            object cur = _serviceProvider.GetRequiredService(handlerType);
            if (!_middlewareMetadata.QueryMiddlewares.Any())
            {
                var method = cur.GetType().GetMethod("HandleAsync");
                var typedTask = (Task<TResult>)method.Invoke(cur, new[] { query });
                return await typedTask;
            }

            for (int i = _middlewareMetadata.QueryMiddlewares.Count - 1; i >= 0; i--)
            {
                var middlewareType = _middlewareMetadata.QueryMiddlewares[i];
                //Todo рефакторинг? добавить вытягивание параметров из сервис-провайдера
                var ctor = middlewareType.GetConstructors().Single();
                cur = ctor.Invoke(new object[] { cur });
            }

            dynamic d = cur;
            var task = (Task)d.HandleAsync(query);
            await task;
            var prop = task.GetType().GetProperty("Result");
            var res = prop.GetValue(task);
            return (TResult) res;

            
        }
    }
}
