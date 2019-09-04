using System.Collections.Generic;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Areas.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Areas.Utils
{
    public static class AreasCqrsRegistrations
    {
        public static void RegisterAreasServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<Area, AreaViewModel>, List<AreaViewModel>>, GetEntitiesQueryHandler<Area, AreaViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Area, int>, Area>, GetEntityQueryHandler<Area, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Area, int>, Area>, GetEntityForDeleteQueryHandler<Area, int>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Area, Area>>, SimpleCreateMappedEntityCommandHandler<Area>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Area, Area, int>>, EditMappedEntityCommandHandler<Area, Area, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Area, int>>, DeleteEntityCommandHandler<Area, int>>();
        }
    }
}
