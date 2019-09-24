using AutoMapper;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Areas.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AreasController : CrudController<int,
        Area, AreaViewModel, Area, Area,
        GetMappedEntitiesQuery<Area, AreaViewModel>, GetEntityByIdQuery<Area, int>, GetEntityByIdForDeleteQuery<Area, int>,
        CreateMappedEntityCommand<Area, Area>, EditMappedEntityCommand<Area, Area, int>, DeleteEntityByIdCommand<Area, int>>
    {
        public AreasController(IHandlerDispatcher dispatcher, IMapper mapper) : base(dispatcher, mapper)
        {            
        }       
    }
}
