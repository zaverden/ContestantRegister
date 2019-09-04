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
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<Area, int>, Area>, GetEntityQueryHandler<Area, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<Area, int>, Area>, GetEntityForDeleteQueryHandler<Area, int>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<Area, Area>>, SimpleCreateMappedEntityCommandHandler<Area>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<Area, Area, int>>, EditMappedEntityCommandHandler<Area, Area, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<Area, int>>, DeleteEntityCommandHandler<Area, int>>();
        }
    }
}
