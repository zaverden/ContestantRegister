using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestantRegister.Framework.Cqrs
{
    public class QueryHandlersPipelineBuilder<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private Type _businessOperationCommandHandlerType;
        private readonly List<Type> _decoratorTypes = new List<Type>();

        public QueryHandlersPipelineBuilder<TQuery, TResult> AddBusinesOperation<TQueryHandler>()
            where TQueryHandler : IQueryHandler<TQuery, TResult>
        {
            _businessOperationCommandHandlerType = typeof(TQueryHandler);

            return this;
        }

        public QueryHandlersPipelineBuilder<TQuery, TResult> AddDecorator<TQueryDecorator>()
            where TQueryDecorator : QueryHandlerDecorator<TQuery, TResult>
        {
            _decoratorTypes.Add(typeof(TQueryDecorator));

            return this;
        }

        protected IQueryHandler<TQuery, TResult> Build(IServiceProvider serviceProvider)
        {
            IQueryHandler<TQuery, TResult> result = null;

            var types = _decoratorTypes.ToList();
            types.Insert(0, _businessOperationCommandHandlerType);

            foreach (var decoratorType in types)
            {
                var ctor = decoratorType.GetConstructors().Single();
                var parameters = ctor.GetParameters();
                var paramValues = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    if (parameter.ParameterType == typeof(IQueryHandler<TQuery, TResult>))
                        paramValues[i] = result;
                    else
                        paramValues[i] = serviceProvider.GetService(parameter.ParameterType);
                }

                result = (IQueryHandler<TQuery, TResult>)ctor.Invoke(paramValues);
            }

            return result;
        }

        public Func<IServiceProvider, IQueryHandler<TQuery, TResult>> BuildFunc()
        {
            if (_businessOperationCommandHandlerType == null) throw new InvalidOperationException("BusinesOperation not configured");

            return Build;
        }
    }
}
