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
    public class RegionsController : CrudController<int,
        Region, RegionViewModel, Region, Region,
        GetMappedEntitiesQuery<Region, RegionViewModel>, GetEntityByIdQuery<Region, int>, GetEntityByIdForDeleteQuery<Region, int>,
        CreateMappedEntityCommand<Region, Region>, EditMappedEntityCommand<Region, Region, int>, DeleteEntityByIdCommand<Region>>
    {
        public RegionsController(IHandlerDispatcher dispatcher, IMapper mapper) : base(dispatcher, mapper)
        {            
        }        
        
    }
}
