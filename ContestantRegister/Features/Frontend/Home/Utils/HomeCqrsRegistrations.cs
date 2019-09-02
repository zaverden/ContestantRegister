using System.Collections.Generic;
using ContestantRegister.Controllers.Account.CommandHandlers;
using ContestantRegister.Controllers.Account.Commands;
using ContestantRegister.Controllers.Account.Queries;
using ContestantRegister.Controllers.Account.QueryHandlers;
using ContestantRegister.Features.Frontend.Account.CommandHandlers;
using ContestantRegister.Features.Frontend.Account.Commands;
using ContestantRegister.Features.Frontend.Home.Commands;
using ContestantRegister.Features.Frontend.Home.CommansHandlers;
using ContestantRegister.Features.Frontend.Home.Queries;
using ContestantRegister.Features.Frontend.Home.QueryHandlers;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Frontend.Account.Utils
{
    public static class HomeCqrsRegistrations
    {
        public static void RegisterHomeServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetContestsForHomeQuery, List<Contest>>, GetContestsForHomeQueryHandler>();
            services.AddTransient<IQueryHandler<GetRegisterParticipantDataQuery, RegisterParticipantData>, GetRegisterParticipantDataQueryHandler>();
            services.AddTransient<IQueryHandler<GetCurrentUserQuery, ApplicationUser>, GetCurrentUserQueryHandler>();
            services.AddTransient<IQueryHandler<GetContestTypeForHomeQuery, ContestType>, GetContestTypeForHomeQueryHandler>();

            services.AddTransient<ICommandHandler<RegisterContestParticipantCommand>, RegisterContestParticipantCommandHandler>();
            services.AddTransient<ICommandHandler<SendEmailCommand>, SendEmailCommandHandler>();

        }
    }
}
