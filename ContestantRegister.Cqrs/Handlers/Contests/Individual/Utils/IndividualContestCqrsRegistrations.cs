using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.QueryHandlers;
using ContestantRegister.Framework.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Utils
{
    public static class IndividualContestCqrsRegistrations
    {
        public static void RegisterIndividualContestServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetExportedIndividualContestParticipantsQuery, ExportIndividualContestParticipantsResult>, GetExportedIndividualContestParticipantsQueryHandler>();
            
            services.AddTransient<ICommandHandler<CreateUserIndividualContestRegistrationCommand>, CreateUserIndividualContestRegistrationCommandHandler>();
            services.AddTransient<ICommandHandler<EditUserIndividualContestRegistrationCommand>, EditUserIndividualContestRegistrationCommandHandler>();

            services.AddTransient<ICommandHandler<CreateAdminIndividualContestRegistrationCommand>, CreateAdminIndividualContestRegistrationCommandHandler>();
            services.AddTransient<ICommandHandler<EditAdminIndividualContestRegistrationCommand>, EditAdminIndividualContestRegistrationCommandHandler>();
        }
    }
}
