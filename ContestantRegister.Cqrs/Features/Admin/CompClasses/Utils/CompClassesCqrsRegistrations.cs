using System.Collections.Generic;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.CompClasses.Queries;
using ContestantRegister.Cqrs.Features.Admin.CompClasses.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.CompClasses.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Admin.CompClasses.Utils
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
