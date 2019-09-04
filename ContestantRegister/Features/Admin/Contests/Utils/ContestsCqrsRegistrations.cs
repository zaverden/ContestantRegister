using System.Collections.Generic;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Contests.CommandHandlers;
using ContestantRegister.Cqrs.Features.Admin.Contests.Queries;
using ContestantRegister.Cqrs.Features.Admin.Contests.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Framework.Cqrs;
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
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Contest, int>, Contest>, GetEntityQueryHandler<Contest, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Contest, int>, Contest>, GetEntityForDeleteQueryHandler<Contest, int>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Contest, ContestDetailsViewModel>>, CreateContestCommandHandler>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Contest, ContestDetailsViewModel, int>>, EditContestCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Contest, int>>, DeleteEntityCommandHandler<Contest, int>>();

        }
    }
}
