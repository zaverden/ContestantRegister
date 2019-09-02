using ContestantRegister.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Controllers.Institutions.ViewModels;
using ContestantRegister.Controllers.Institutions.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class InstitutionsController : CrudController<
        Institution, InstitutionListItemViewModel,
        GetEntitiesWithMappingQuery<Institution, InstitutionListItemViewModel>, GetEntityByIdQuery<Institution>, GetEntityByIdForDeleteQuery<Institution>,
        CreateEntityCommand<Institution>, EditEntityCommand<Institution>, DeleteEntityByIdCommand<Institution>>
    {
        public InstitutionsController(IHandlerDispatcher handlerDispatcher) : base(handlerDispatcher)
        {            
        }

        protected override async Task FillViewDataDetailFormAsync(Institution item = null)
        {
            var cities = await HandlerDispatcher.ExecuteQueryAsync(new CitiesForInstitutionQuery());
            ViewData["CityId"] = new SelectList(cities, "Id", "Name", item?.CityId);
        }
        
    }
}
