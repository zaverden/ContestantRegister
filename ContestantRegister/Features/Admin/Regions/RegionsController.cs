using AutoMapper;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Controllers.Regions;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class RegionsController : CrudController<Region, RegionViewModel, Region,
        GetMappedEntitiesQuery<Region, RegionViewModel>, GetEntityByIdQuery<Region>, GetEntityByIdForDeleteQuery<Region>,
        CreateMappedEntityCommand<Region, Region>, EditMappedEntityCommand<Region, Region>, DeleteEntityByIdCommand<Region>>
    {
        public RegionsController(IHandlerDispatcher dispatcher, IMapper mapper) : base(dispatcher, mapper)
        {            
        }        
        
    }
}
