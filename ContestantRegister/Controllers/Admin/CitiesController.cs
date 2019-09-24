using AutoMapper;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Cities.Queries;
using ContestantRegister.Cqrs.Features.Admin.Cities.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CitiesController : CrudController<int,
            City, CityListItemViewModel, City, City,
            GetMappedEntitiesQuery<City, CityListItemViewModel>, GetEntityByIdQuery<City, int>, GetEntityByIdForDeleteQuery<City, int>,
            CreateMappedEntityCommand<City, City>, EditMappedEntityCommand<City, City, int>, DeleteEntityByIdCommand<City, int>>
    {
        public CitiesController(IHandlerDispatcher dispatcher, IMapper mapper) : base(dispatcher, mapper)
        {
        }

        protected override async Task FillViewDataForCreateAsync(City viewModel = null)
        {
            await FillViewDataDetailFormAsync(viewModel);
        }

        protected override async Task FillViewDataForEditAsync(City viewModel = null)
        {
            await FillViewDataDetailFormAsync(viewModel);
        }

        private async Task FillViewDataDetailFormAsync(City item = null)
        {
            var regions = await HandlerDispatcher.ExecuteQueryAsync(new GetRegionsForCityQuery());
            ViewData["RegionId"] = new SelectList(regions, "Id", "Name", item?.RegionId);
        }        

    }
}
