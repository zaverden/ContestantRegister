using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.QueryHandlers;
using ContestantRegister.Framework.Cqrs;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Utils
{
    public static class TeamContestCqrsRegistrations
    {
        public static void RegisterTeamContestServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetImportBaylorRegistrationDataQuery, ImportParticipantsViewModel>, GetImportBaylorRegistrationDataQueryHandler>();
            services.AddTransient<IQueryHandler<GetExportedTeamsForContestQuery, ExportTeamsResult>, GetExportedTeamsForContestQueryHandler>();
            services.AddTransient<IQueryHandler<GetExportedTeamContestParticipantsQuery, ExcelPackage>, GetExportedTeamContestParticipantsQueryHandler>();
            services.AddTransient<IQueryHandler<GetDataForContestRegistrationQuery, DataForContestRegistration>, GetDataForContestRegistrationQueryHandler>();

            services.AddTransient<ICommandHandler<ImportBaylorRegistrationsCommand>, ImportBaylorRegistrationsCommandHandler>();
            services.AddTransient<ICommandHandler<CreateTeamContestRegistrationCommand>, CreateTeamContestRegistrationCommandHandler>();
            services.AddTransient<ICommandHandler<EditTeamContestRegistrationCommand>, EditTeamContestRegistrationCommandHandler>();
        }
    }
}
