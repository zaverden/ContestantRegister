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
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Region>, Region>, GetEntityQueryHandler<Region>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Region>, Region>, GetEntityForDeleteQueryHandler<Region>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Region, Region>>, CreateMappedEntityCommandHandler<Region, Region>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Region, Region>>, EditMappedEntityCommandHandler<Region, Region>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Region>>, DeleteEntityCommandHandler<Region>>();

        }
    }
}
