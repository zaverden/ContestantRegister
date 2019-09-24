using System.Collections.Generic;
using ContestantRegister.Cqrs.Features.Frontend.Home.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Home.CommansHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Home.QueryHandlers;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Utils
{
    public static class HomeCqrsRegistrations
    {
        public static void RegisterHomeServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetContestsForHomeQuery, List<Contest>>, GetContestsForHomeQueryHandler>();
            services.AddTransient<IQueryHandler<GetRegisterParticipantDataQuery, RegisterParticipantData>, GetRegisterParticipantDataQueryHandler>();
            services.AddTransient<IQueryHandler<GetCurrentUserQuery, ApplicationUser>, GetCurrentUserQueryHandler>();
            services.AddTransient<IQueryHandler<GetContestTypeForHomeQuery, ContestType>, GetContestTypeForHomeQueryHandler>();
            services.AddTransient<IQueryHandler<GetUserForDetailsQuery, ApplicationUser>, GetUserForDetailsQueryHandler>();
            
            services.AddTransient<ICommandHandler<RegisterContestParticipantCommand>, RegisterContestParticipantCommandHandler>();
            services.AddTransient<ICommandHandler<SendEmailCommand>, SendEmailCommandHandler>();

        }
    }
}
