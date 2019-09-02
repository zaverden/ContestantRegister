using System.Collections.Generic;
using ContestantRegister.Controllers;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Institutions.Queries;
using ContestantRegister.Controllers.Institutions.ViewModels;
using ContestantRegister.Controllers.Regions;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Institutions.Utils
{
    public static class RegionsCqrsRegistrations
    {
        public static void RegisterRegionsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetEntitiesWithMappingQuery<Region, RegionViewModel>, List<RegionViewModel>>, GetEntitiesQueryHandler<Region, RegionViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Region>, Region>, GetEntityQueryHandler<Region>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Region>, Region>, GetEntityForDeleteQueryHandler<Region>>();

            services.AddTransient<ICommandHandler<CreateEntityCommand<Region>>, CreateEntityCommandHandler<Region>>();
            services.AddTransient<ICommandHandler<EditEntityCommand<Region>>, EditEntityCommandHandler<Region>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Region>>, DeleteEntityCommandHandler<Region>>();

        }
    }
}
