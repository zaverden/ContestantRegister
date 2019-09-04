using System.Collections.Generic;
using ContestantRegister.Controllers;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Institutions.Queries;
using ContestantRegister.Controllers.Institutions.ViewModels;
using ContestantRegister.Controllers.Regions;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Institutions.Utils
{
    public static class RegionsCqrsRegistrations
    {
        public static void RegisterRegionsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<Region, RegionViewModel>, List<RegionViewModel>>, GetEntitiesQueryHandler<Region, RegionViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Region, int>, Region>, GetEntityQueryHandler<Region, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Region, int>, Region>, GetEntityForDeleteQueryHandler<Region, int>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Region, Region>>, SimpleCreateMappedEntityCommandHandler<Region>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Region, Region, int>>, EditMappedEntityCommandHandler<Region, Region, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Region, int>>, DeleteEntityCommandHandler<Region, int>>();

        }
    }
}
