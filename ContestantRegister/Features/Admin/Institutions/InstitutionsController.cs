using ContestantRegister.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Controllers.Institutions.ViewModels;
using ContestantRegister.Controllers.Institutions.Queries;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class InstitutionsController : CrudController<
        Institution, InstitutionListItemViewModel, Institution,
        GetMappedEntitiesQuery<Institution, InstitutionListItemViewModel>, GetEntityByIdQuery<Institution>, GetEntityByIdForDeleteQuery<Institution>,
        CreateMappedEntityCommand<Institution, Institution>, EditMappedEntityCommand<Institution, Institution>, DeleteEntityByIdCommand<Institution>>
    {
        public InstitutionsController(IHandlerDispatcher handlerDispatcher, IMapper mapper) : base(handlerDispatcher, mapper)
        {            
        }

        protected override async Task FillViewDataDetailFormAsync(Institution item = null)
        {
            var cities = await HandlerDispatcher.ExecuteQueryAsync(new CitiesForInstitutionQuery());
            ViewData["CityId"] = new SelectList(cities, "Id", "Name", item?.CityId);
        }

        protected override string[] GetIncludePropertiesForDelete()
        {
            return new[] {nameof(Institution.City)};
        }
    }
}
