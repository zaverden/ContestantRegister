using System.Collections.Generic;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Institutions.Queries;
using ContestantRegister.Cqrs.Features.Admin.Institutions.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Institutions.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Admin.Institutions.Utils
{
    public static class InstitutionsCqrsRegistrations
    {
        public static void RegisterInstitutionsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<Institution, InstitutionListItemViewModel>, List<InstitutionListItemViewModel>>, GetEntitiesQueryHandler<Institution, InstitutionListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Institution, int>, Institution>, GetEntityQueryHandler<Institution, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Institution, int>, Institution>, GetEntityForDeleteQueryHandler<Institution, int>>();

            services.AddTransient<IQueryHandler<CitiesForInstitutionQuery, List<City>>, GetCitiesForInstitutionQueryHandler>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Institution, Institution>>, SimpleCreateMappedEntityCommandHandler<Institution>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Institution, Institution, int>>, EditMappedEntityCommandHandler<Institution, Institution, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Institution, int>>, DeleteEntityCommandHandler<Institution, int>>();
        }
    }
}
