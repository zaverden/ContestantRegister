using System.Linq;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContestantRegister.Controllers.Cities;
using ContestantRegister.Controllers.Cities.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CitiesController : CrudController<City, CityListItemViewModel,
            GetEntitiesWithMappingQuery<City, CityListItemViewModel>, GetEntityByIdQuery<City>, GetEntityByIdForDeleteQuery<City>,
            CreateEntityCommand<City>, EditEntityCommand<City>, DeleteEntityByIdCommand<City>>
    {
        public CitiesController(IHandlerDispatcher dispatcher) : base(dispatcher)
        {
        }

        protected override async Task FillViewDataDetailFormAsync(City item = null)
        {
            var regions = await HandlerDispatcher.ExecuteQueryAsync(new GetRegionsForCityQuery());
            ViewData["RegionId"] = new SelectList(regions, "Id", "Name", item?.RegionId);
        }        

    }
}
