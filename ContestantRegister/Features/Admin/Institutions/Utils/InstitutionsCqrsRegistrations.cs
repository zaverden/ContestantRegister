using System.Collections.Generic;
using ContestantRegister.Controllers;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Institutions.Queries;
using ContestantRegister.Controllers.Institutions.ViewModels;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Institutions.Utils
{
    public static class InstitutionsCqrsRegistrations
    {
        public static void RegisterInstitutionsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<Institution, InstitutionListItemViewModel>, List<InstitutionListItemViewModel>>, GetEntitiesQueryHandler<Institution, InstitutionListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Institution>, Institution>, GetEntityQueryHandler<Institution>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Institution>, Institution>, GetEntityForDeleteQueryHandler<Institution>>();

            services.AddTransient<IQueryHandler<CitiesForInstitutionQuery, List<City>>, GetCitiesForInstitutionQueryHandler>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Institution, Institution>>, CreateMappedEntityCommandHandler<Institution, Institution>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Institution, Institution>>, EditMappedEntityCommandHandler<Institution, Institution>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Institution>>, DeleteEntityCommandHandler<Institution>>();
        }
    }
}
