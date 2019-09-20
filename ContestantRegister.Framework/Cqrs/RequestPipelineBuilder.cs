using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestantRegister.Framework.Cqrs
{
    public class RequestPipelineBuilder<TRequestHandler, TRequestDecorator>
    {
        private Type _businessOperationCommandHandlerType;
        private readonly List<Type> _decoratorTypes = new List<Type>();

        public RequestPipelineBuilder<TRequestHandler, TRequestDecorator> AddBusinesOperation<THandler>()
            where THandler : TRequestHandler
        {
            if (_businessOperationCommandHandlerType != null) throw new InvalidOperationException("BusinesOperation already added");

            _businessOperationCommandHandlerType = typeof(THandler);

            return this;
        }

        public RequestPipelineBuilder<TRequestHandler, TRequestDecorator> AddDecorator<TDecorator>()
            where TDecorator : TRequestDecorator
        {
            if (_businessOperationCommandHandlerType == null) throw new InvalidOperationException("Add BusinesOperation before decorators");

            _decoratorTypes.Add(typeof(TDecorator));

            return this;
        }

        public Func<IServiceProvider, TRequestHandler> BuildFunc()
        {
            if (_businessOperationCommandHandlerType == null) throw new InvalidOperationException("BusinesOperation not configured");

            return Build;
        }

        protected TRequestHandler Build(IServiceProvider serviceProvider)
        {
            TRequestHandler result = default;

            var types = _decoratorTypes.ToList();//создается отдельный список, чтобы не менять поле, добавляя бизнес-операцию
            types.Insert(0, _businessOperationCommandHandlerType);

            //нельзя менять на foreach, ибо он не гарантирует порядок перебора, а порядок добавления декораторов важен
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < types.Count; i++)
            {
                var ctor = types[i].GetConstructors().Single();
                var parameters = ctor.GetParameters();
                var paramValues = new object[parameters.Length];
                for (int j = 0; j < parameters.Length; j++)
                {
                    var parameter = parameters[j];
                    if (parameter.ParameterType == typeof(TRequestHandler))
                        paramValues[j] = result;
                    else
                        paramValues[j] = serviceProvider.GetService(parameter.ParameterType);
                }

                result = (TRequestHandler)ctor.Invoke(paramValues);
            }

            return result;
        }
    }
}
