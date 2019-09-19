using System;
using System.Collections.Generic;
using System.Text;

namespace ContestantRegister.Framework.Cqrs
{
    public class MiddlewareMetadata
    {
        private readonly List<Type> _commandMiddlewareTypes = new List<Type>();
        private readonly List<Type> _queryMiddlewareTypes = new List<Type>();

        public void AddCommandMiddleware<TMiddleware>() where TMiddleware : CommandHandlerMiddleware
        {
            _commandMiddlewareTypes.Add(typeof(TMiddleware));
        }

        //можно заменить тип возвращаемого результата на массив, ибо он недолжен меняться
        public IList<Type> CommandMiddlewares => _commandMiddlewareTypes;

        public void AddQueryMiddleware<TMiddleware>() where TMiddleware : QueryHandlerMiddleware
        {
            _queryMiddlewareTypes.Add(typeof(TMiddleware));
        }

        public IList<Type> QueryMiddlewares => _queryMiddlewareTypes;
    }
}
