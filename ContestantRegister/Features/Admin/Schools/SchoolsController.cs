using ContestantRegister.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Schools.Queries;
using ContestantRegister.Cqrs.Features.Admin.Schools.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class SchoolsController : CrudController<int,
        School, SchoolListItemViewModel, School, School,
        GetMappedSchoolsQuery, GetEntityByIdQuery<School, int>, GetEntityByIdForDeleteQuery<School, int>,
        CreateMappedEntityCommand<School, School>, EditMappedEntityCommand<School, School, int>, DeleteEntityByIdCommand<School, int>>
    {
        public SchoolsController(IHandlerDispatcher handlerDispatcher, IMapper mapper) : base(handlerDispatcher, mapper)
        {            
        }

        protected override async Task FillViewDataForEditAsync(School viewModel = null)
        {
            await FillViewDataDetailFormAsync(viewModel);
        }

        protected override async Task FillViewDataForCreateAsync(School viewModel = null)
        {
            await FillViewDataDetailFormAsync(viewModel);
        }

        private async Task FillViewDataDetailFormAsync(School item)
        {
            var cities = await HandlerDispatcher.ExecuteQueryAsync(new CitiesForSchoolQuery());
            ViewData["CityId"] = new SelectList(cities, "Id", "Name", item?.CityId);
        }

        protected override string[] GetIncludePropertiesForDelete()
        {
            return new[] {nameof(School.City)};
        }
    }
}
