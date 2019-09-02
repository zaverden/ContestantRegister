using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Controllers.Areas.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AreasController : CrudController<
        Area, AreaViewModel,
        GetEntitiesWithMappingQuery<Area, AreaViewModel>, GetEntityByIdQuery<Area>, GetEntityByIdForDeleteQuery<Area>,
        CreateEntityCommand<Area>, EditEntityCommand<Area>, DeleteEntityByIdCommand<Area>>
    {
        public AreasController(IHandlerDispatcher dispatcher) : base(dispatcher)
        {            
        }       
    }
}
