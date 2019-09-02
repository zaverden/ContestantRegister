using System;
using ContestantRegister.Infrastructure.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Infrastructure
{
    public class MsDiHandlerDispatcher : HandlerDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public MsDiHandlerDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        protected override object GetService(Type serviceType)
        {
            return _serviceProvider.GetRequiredService(serviceType);
        }
    }
}
