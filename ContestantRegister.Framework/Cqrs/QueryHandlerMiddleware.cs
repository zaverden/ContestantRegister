using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    public abstract class QueryHandlerMiddleware : IQueryHandler<IQuery<object>, object>
    {
        protected readonly object Next;

        protected QueryHandlerMiddleware(object next)
        {
            Next = next;
        }

        public abstract Task<object> HandleAsync(IQuery<object> query);
        
    }
}
