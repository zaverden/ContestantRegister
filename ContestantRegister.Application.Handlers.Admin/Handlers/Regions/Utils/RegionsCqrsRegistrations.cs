using System.Collections.Generic;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Regions.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Admin.Regions.Utils
{
    public static class RegionsCqrsRegistrations
    {
        public static void RegisterRegionsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<Region, RegionViewModel>, List<RegionViewModel>>, GetEntitiesQueryHandler<Region, RegionViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Region, int>, Region>, GetEntityByIdQueryHandler<Region, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Region, int>, Region>, GetEntityByIdForDeleteQueryHandler<Region, int>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Region, Region>>, SimpleCreateMappedEntityCommandHandler<Region>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Region, Region, int>>, EditMappedEntityCommandHandler<Region, Region, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Region, int>>, DeleteEntityByIdCommandHandler<Region, int>>();

        }
    }
}
