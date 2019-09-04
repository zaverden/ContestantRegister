using System.Collections.Generic;
using ContestantRegister.Cqrs.Features.Admin.Emails.Queries;
using ContestantRegister.Cqrs.Features.Admin.Emails.QueryHandlers;
using ContestantRegister.Framework.Cqrs;
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
