using ContestantRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Contests.Queries;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister
{
    [Authorize(Roles = Roles.Admin)]
    public class ContestsController : CrudController<
        Contest, ContestListItemViewModel, ContestDetailsViewModel,
        GetMappedEntitiesQuery<Contest, ContestListItemViewModel>, GetEntityByIdQuery<Contest>, GetEntityByIdForDeleteQuery<Contest>,
        CreateMappedEntityCommand<Contest, ContestDetailsViewModel>, EditMappedEntityCommand<Contest, ContestDetailsViewModel>, DeleteEntityByIdCommand<Contest>>
    {
        public ContestsController(IHandlerDispatcher handlerDispatcher, IMapper mapper) : base(handlerDispatcher, mapper)
        {
        }

        protected override async Task FillViewDataDetailFormAsync(ContestDetailsViewModel viewModel = null)
        {
            var areas = await HandlerDispatcher.ExecuteQueryAsync(new GetAreasForContestQuery());
            ViewData["Areas"] = new MultiSelectList(areas, "Id", "Name", viewModel?.ContestAreas.Select(c => c.AreaId));
        }
        
        protected override string[] GetIncludePropertiesForEdit()
        {
            return new[] {nameof(Contest.ContestAreas)};
        }
    }
}
