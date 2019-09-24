using AutoMapper;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Regions.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class RegionsController : CrudController<int,
        Region, RegionViewModel, Region, Region,
        GetMappedEntitiesQuery<Region, RegionViewModel>, GetEntityByIdQuery<Region, int>, GetEntityByIdForDeleteQuery<Region, int>,
        CreateMappedEntityCommand<Region, Region>, EditMappedEntityCommand<Region, Region, int>, DeleteEntityByIdCommand<Region, int>>
    {
        public RegionsController(IHandlerDispatcher dispatcher, IMapper mapper) : base(dispatcher, mapper)
        {            
        }        
        
    }
}
