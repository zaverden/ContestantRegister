using System.Collections.Generic;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Cities;
using ContestantRegister.Controllers.Cities.Queries;
using ContestantRegister.Controllers.CompClasses;
using ContestantRegister.Controllers.CompClasses.Queries;
using ContestantRegister.Controllers.CompClasses.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Emails.Queries;
using ContestantRegister.Cqrs.Features.Admin.Emails.QueryHandlers;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Admin.Cities.Utils
{
    public static class EmailsCqrsRegistrations
    {
        public static void RegisterEmailsServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetEmailsQuery, List<Email>>, GetEmailsQueryHandler>();
            
        }
    }
}
