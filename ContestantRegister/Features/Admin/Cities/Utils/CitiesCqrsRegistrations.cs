using System.Collections.Generic;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Cities;
using ContestantRegister.Controllers.Cities.Queries;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Cities.Utils
{
    public static class CitiesCqrsRegistrations
    {
        public static void RegisterCitiesServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<City, CityListItemViewModel>, List<CityListItemViewModel>>, GetEntitiesQueryHandler<City, CityListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<City, int>, City>, GetEntityQueryHandler<City, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<City, int>, City>, GetEntityForDeleteQueryHandler<City, int>>();

            services.AddTransient<IQueryHandler<GetRegionsForCityQuery, List<Region>>, GetRegionsForCityQueryHandler>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<City, City>>, SimpleCreateMappedEntityCommandHandler<City>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<City, City, int>>, EditMappedEntityCommandHandler<City, City, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<City, int>>, DeleteEntityCommandHandler<City, int>>();

        }
    }
}
