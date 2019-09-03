using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AreasController : CrudController<int,
        Area, AreaViewModel, Area, Area,
        GetMappedEntitiesQuery<Area, AreaViewModel>, GetEntityByIdQuery<Area, int>, GetEntityByIdForDeleteQuery<Area, int>,
        CreateMappedEntityCommand<Area, Area>, EditMappedEntityCommand<Area, Area, int>, DeleteEntityByIdCommand<Area>>
    {
        public AreasController(IHandlerDispatcher dispatcher, IMapper mapper) : base(dispatcher, mapper)
        {            
        }       
    }
}
