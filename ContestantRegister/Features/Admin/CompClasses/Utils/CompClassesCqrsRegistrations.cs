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
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<CompClass, int>, CompClass>, GetEntityQueryHandler<CompClass, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<CompClass, int>, CompClass>, GetEntityForDeleteQueryHandler<CompClass, int>>();

            services.AddTransient<IQueryHandler<GetAreasForCompClassQuery, List<Area>>, GetAreasForCompClassQueryHandler>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<CompClass, CompClass>>, SimpleCreateMappedEntityCommandHandler<CompClass>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<CompClass, CompClass, int>>, EditMappedEntityCommandHandler<CompClass, CompClass, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<CompClass, int>>, DeleteEntityCommandHandler<CompClass, int>>();

        }
    }
}
