using System.Collections.Generic;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Schools;
using ContestantRegister.Controllers.Schools.Queries;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Schools.Utils
{
    public static class SchoolsCqrsRegistrations
    {
        public static void RegisterSchoolsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetMappedSchoolsQuery, List<SchoolListItemViewModel>>, GetEntitiesQueryHandler<School, SchoolListItemViewModel>>();
            services.AddTransient<IQueryHandler<CitiesForSchoolQuery, List<City>>, GetCitiesForSchoolQueryHandler>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<School, int>, School>, GetEntityQueryHandler<School, int>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<School, int>, School>, GetEntityForDeleteQueryHandler<School, int>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<School, School>>, SimpleCreateMappedEntityCommandHandler<School>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<School, School, int>>, EditMappedEntityCommandHandler<School, School, int>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<School, int>>, DeleteEntityCommandHandler<School, int>>();
        }
    }
}
