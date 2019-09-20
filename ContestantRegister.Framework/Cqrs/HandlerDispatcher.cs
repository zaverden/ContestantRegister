using System;
using System.Collections.Generic;
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

            cur = AddMiddlevares(cur, _middlewareMetadata.CommandMiddlewares);

            //больше люблю reflection, чем dynamic
            object res = cur.GetType().GetMethod("HandleAsync").Invoke(cur, new object[]{command});
            
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

            cur = AddMiddlevares(cur, _middlewareMetadata.QueryMiddlewares);

            //ради сравнения тут dynamic, а не reflection
            dynamic d = cur; // обойтись без reflection не удастся, так как тип query не известен
            var task = (Task)d.HandleAsync(query);
            await task;
            var res = ((dynamic) task).Result;
            return (TResult) res;
        }

        private object AddMiddlevares(object handler, IList<Type> middlewares)
        {
            var cur = handler;
            for (int i = middlewares.Count - 1; i >= 0; i--)
            {
                var ctor = middlewares[i].GetConstructors().Single();
                var parameters = ctor.GetParameters();
                var paramValues = new object[parameters.Length];
                for (int j = 0; j < parameters.Length; j++)
                {
                    var parameter = parameters[j];
                    if (parameter.ParameterType == typeof(object))
                        paramValues[j] = cur;
                    else
                        paramValues[j] = _serviceProvider.GetService(parameter.ParameterType);
                }
                cur = ctor.Invoke(paramValues);
            }

            return cur;
        }
    }
}
