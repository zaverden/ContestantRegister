using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common
{
    public abstract class GetController<TEntity, TListQuery> : Controller
    {
        protected readonly IHandlerDispatcher HandlerDispatcher;

        protected GetController(IHandlerDispatcher dispatcher)
        {
            HandlerDispatcher = dispatcher;
        }
    }
}
