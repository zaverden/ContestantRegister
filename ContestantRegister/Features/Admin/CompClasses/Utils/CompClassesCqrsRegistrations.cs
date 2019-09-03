using System.Collections.Generic;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Cities;
using ContestantRegister.Controllers.Cities.Queries;
using ContestantRegister.Controllers.CompClasses;
using ContestantRegister.Controllers.CompClasses.Queries;
using ContestantRegister.Controllers.CompClasses.QueryHandlers;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Cities.Utils
{
    public static class CompClassesCqrsRegistrations
    {
        public static void RegisterCompClassesServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<CompClass, CompClassListItemViewModel>, List<CompClassListItemViewModel>>, GetEntitiesQueryHandler<CompClass, CompClassListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<CompClass>, CompClass>, GetEntityQueryHandler<CompClass>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<CompClass>, CompClass>, GetEntityForDeleteQueryHandler<CompClass>>();

            services.AddTransient<IQueryHandler<GetAreasForCompClassQuery, List<Area>>, GetAreasForCompClassQueryHandler>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<CompClass, CompClass>>, CreateMappedEntityCommandHandler<CompClass, CompClass>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<CompClass, CompClass>>, EditMappedEntityCommandHandler<CompClass, CompClass>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<CompClass>>, DeleteEntityCommandHandler<CompClass>>();

        }
    }
}
