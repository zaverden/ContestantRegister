using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Areas.ViewModels;
using ContestantRegister.Controllers.Cities;
using ContestantRegister.Controllers.Cities.Queries;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Areas.Utils
{
    public static class AreasCqrsRegistrations
    {
        public static void RegisterAreasServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedEntitiesQuery<Area, AreaViewModel>, List<AreaViewModel>>, GetEntitiesQueryHandler<Area, AreaViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Area>, Area>, GetEntityQueryHandler<Area>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Area>, Area>, GetEntityForDeleteQueryHandler<Area>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Area, Area>>, CreateMappedEntityCommandHandler<Area, Area>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Area, Area>>, EditMappedEntityCommandHandler<Area, Area>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Area>>, DeleteEntityCommandHandler<Area>>();
        }
    }
}
