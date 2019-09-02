using System.Collections.Generic;
using ContestantRegister.Controllers;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Institutions.Queries;
using ContestantRegister.Controllers.Institutions.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Institutions.Utils
{
    public static class InstitutionsCqrsRegistrations
    {
        public static void RegisterInstitutionsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetEntitiesWithMappingQuery<Institution, InstitutionListItemViewModel>, List<InstitutionListItemViewModel>>, GetEntitiesQueryHandler<Institution, InstitutionListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Institution>, Institution>, GetEntityQueryHandler<Institution>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Institution>, Institution>, GetInstitutionForDeleteQueryHandler>();

            services.AddTransient<IQueryHandler<CitiesForInstitutionQuery, List<City>>, GetCitiesForInstitutionQueryHandler>();

            services.AddTransient<ICommandHandler<CreateEntityCommand<Institution>>, CreateEntityCommandHandler<Institution>>();
            services.AddTransient<ICommandHandler<EditEntityCommand<Institution>>, EditEntityCommandHandler<Institution>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Institution>>, DeleteEntityCommandHandler<Institution>>();
        }
    }
}
