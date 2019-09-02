using ContestantRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Controllers.Schools;
using ContestantRegister.Controllers.Schools.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Utils.Exceptions;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class SchoolsController : CrudController<
        School, SchoolListItemViewModel,
        GetSchoolsQuery, GetEntityByIdQuery<School>, GetEntityByIdForDeleteQuery<School>,
        CreateEntityCommand<School>, EditEntityCommand<School>, DeleteEntityByIdCommand<School>>
    {
        public SchoolsController(IHandlerDispatcher handlerDispatcher) : base(handlerDispatcher)
        {            
        }

        protected override async Task FillViewDataDetailFormAsync(School item)
        {
            var cities = await HandlerDispatcher.ExecuteQueryAsync(new CitiesForSchoolQuery());
            ViewData["CityId"] = new SelectList(cities, "Id", "Name", item?.CityId);
        }
        
    }
}
