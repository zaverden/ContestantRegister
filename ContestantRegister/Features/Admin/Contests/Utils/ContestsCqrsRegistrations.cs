using System.Collections.Generic;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Regions;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Contests.CommandHandlers;
using ContestantRegister.Cqrs.Features.Admin.Contests.Queries;
using ContestantRegister.Cqrs.Features.Admin.Contests.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Cities.Utils
{
    public static class ContestsCqrsRegistrations
    {
        public static void RegisterContestsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetAreasForContestQuery, List<Area>>, GetAreasForContestQueryHandler>();

            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<Contest, ContestListItemViewModel>, List<ContestListItemViewModel>>, GetEntitiesQueryHandler<Contest, ContestListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Contest>, Contest>, GetEntityQueryHandler<Contest>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Contest>, Contest>, GetEntityForDeleteQueryHandler<Contest>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Contest, ContestDetailsViewModel>>, CreateContestCommandHandler>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Contest, ContestDetailsViewModel>>, EditContestCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Contest>>, DeleteEntityCommandHandler<Contest>>();

        }
    }
}
