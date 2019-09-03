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
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<School>, School>, GetEntityQueryHandler<School>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<School>, School>, GetEntityForDeleteQueryHandler<School>>();

            services.AddTransient<ICommandHandler<CreateMappedEntityCommand<School, School>>, CreateMappedEntityCommandHandler<School, School>>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<School, School>>, EditMappedEntityCommandHandler<School, School>>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<School>>, DeleteEntityCommandHandler<School>>();
        }
    }
}
