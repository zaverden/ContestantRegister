using System.Collections.Generic;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Cities.Queries;
using ContestantRegister.Cqrs.Features.Admin.Cities.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Cities.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Admin.Cities.Utils
{
    public static class CitiesCqrsRegistrations
    {
        public static void RegisterCitiesServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<City, CityListItemViewModel>, List<CityListItemViewModel>>, GetEntitiesQueryHandler<City, CityListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<City, int>, City>, GetEntityByIdQueryHandler<City, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<City, int>, City>, GetEntityByIdForDeleteQueryHandler<City, int>>();

            services.AddTransient<IQueryHandler<GetRegionsForCityQuery, List<Region>>, GetRegionsForCityQueryHandler>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<City, City>>, SimpleCreateMappedEntityCommandHandler<City>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<City, City, int>>, EditMappedEntityCommandHandler<City, City, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<City, int>>, DeleteEntityByIdCommandHandler<City, int>>();

        }
    }
}
