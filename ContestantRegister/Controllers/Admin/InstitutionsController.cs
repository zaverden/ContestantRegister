﻿using ContestantRegister.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common;
using Microsoft.AspNetCore.Authorization;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Institutions.Queries;
using ContestantRegister.Cqrs.Features.Admin.Institutions.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class InstitutionsController : CrudController<int,
        Institution, InstitutionListItemViewModel, Institution, Institution,
        GetMappedEntitiesQuery<Institution, InstitutionListItemViewModel>, GetEntityByIdQuery<Institution, int>, GetEntityByIdForDeleteQuery<Institution, int>,
        CreateMappedEntityCommand<Institution, Institution>, EditMappedEntityCommand<Institution, Institution, int>, DeleteEntityByIdCommand<Institution, int>>
    {
        public InstitutionsController(IHandlerDispatcher handlerDispatcher, IMapper mapper) : base(handlerDispatcher, mapper)
        {            
        }

        protected override async Task FillViewDataForCreateAsync(Institution viewModel = null)
        {
            await FillViewDataDetailFormAsync(viewModel);
        }

        protected override async Task FillViewDataForEditAsync(Institution viewModel = null)
        {
            await FillViewDataDetailFormAsync(viewModel);
        }

        private async Task FillViewDataDetailFormAsync(Institution item = null)
        {
            var cities = await HandlerDispatcher.ExecuteQueryAsync(new GetCitiesForInstitutionQuery());
            ViewData["CityId"] = new SelectList(cities, "Id", "Name", item?.CityId);
        }

        protected override string[] GetIncludePropertiesForDelete()
        {
            return new[] {nameof(Institution.City)};
        }
    }
}
