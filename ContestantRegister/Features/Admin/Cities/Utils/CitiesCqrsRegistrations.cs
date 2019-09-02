using System.Collections.Generic;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Cities;
using ContestantRegister.Controllers.Cities.Queries;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Cities.Utils
{
    public static class CitiesCqrsRegistrations
    {
        public static void RegisterCitiesServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetEntitiesWithMappingQuery<City, CityListItemViewModel>, List<CityListItemViewModel>>, GetEntitiesQueryHandler<City, CityListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<City>, City>, GetEntityQueryHandler<City>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<City>, City>, GetEntityForDeleteQueryHandler<City>>();

            services.AddTransient<IQueryHandler<GetRegionsForCityQuery, List<Region>>, GetRegionsForCityQueryHandler>();

            services.AddTransient<ICommandHandler<CreateEntityCommand<City>>, CreateEntityCommandHandler<City>>();
            services.AddTransient<ICommandHandler<EditEntityCommand<City>>, EditEntityCommandHandler<City>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<City>>, DeleteEntityCommandHandler<City>>();

        }
    }
}
