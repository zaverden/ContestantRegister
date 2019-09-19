using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestantRegister.Framework.Cqrs
{
    public class CommandHandlersPipelineBuilder<TCommand> where TCommand : ICommand
    {
        private Type _businessOperationCommandHandlerType;
        private readonly List<Type> _decoratorTypes = new List<Type>();

        public CommandHandlersPipelineBuilder<TCommand> AddBusinesOperation<TCommandHandler>()
            where TCommandHandler : ICommandHandler<TCommand>
        {
            _businessOperationCommandHandlerType = typeof(TCommandHandler);

            return this;
        }

        public CommandHandlersPipelineBuilder<TCommand> AddDecorator<TCommandDecorator>()
            where TCommandDecorator : CommandHandlerDecorator<TCommand>
        {
            _decoratorTypes.Add(typeof(TCommandDecorator));

            return this;
        }

        protected ICommandHandler<TCommand> Build(IServiceProvider serviceProvider)
        {
            ICommandHandler<TCommand> result = null;

            var types = _decoratorTypes.ToList();//создается отдельный список, чтобы не менять поле, добавляя бизнес-операцию
            types.Insert(0, _businessOperationCommandHandlerType);

            foreach (var decoratorType in types)
            {
                var ctor = decoratorType.GetConstructors().Single();
                var parameters = ctor.GetParameters();
                var paramValues = new object[parameters.Length];
                for(int i=0; i< parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    if (parameter.ParameterType == typeof(ICommandHandler<TCommand>))
                        paramValues[i] = result;
                    else
                        paramValues[i] = serviceProvider.GetService(parameter.ParameterType);
                }

                result = (ICommandHandler<TCommand>)ctor.Invoke(paramValues);
            }

            return result;
        }

        public Func<IServiceProvider, ICommandHandler<TCommand>> BuildFunc()
        {
            if (_businessOperationCommandHandlerType == null) throw new InvalidOperationException("BusinesOperation not configured");

            return Build;
        }
    }
}
