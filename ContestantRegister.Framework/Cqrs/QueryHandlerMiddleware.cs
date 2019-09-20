using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    public abstract class QueryHandlerMiddleware : IQueryHandler<IQuery<object>, object>
    {
        private readonly object _next;

        protected QueryHandlerMiddleware(object next)
        {
            _next = next;
        }

        protected async Task<object> HandleNextAsync(IQuery<object> query)
        {
            var handlemethod = _next.GetType().GetMethod("HandleAsync");
            var task = (Task)handlemethod.Invoke(_next, new[] { query });
            await task;
            var prop = task.GetType().GetProperty("Result");
            var res = prop.GetValue(task);
            return res;
        }

        public abstract Task<object> HandleAsync(IQuery<object> query);
        
    }
}
