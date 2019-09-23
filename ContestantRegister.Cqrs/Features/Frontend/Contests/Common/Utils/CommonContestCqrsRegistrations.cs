using System.Collections.Generic;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Utils
{
    public static class CommonContestCqrsRegistrations
    {
        public static void RegisterCommonContestServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<ImportParticipantsQuery, ImportParticipantsViewModel>, ImportParticipantsQueryHandler>();
            services.AddTransient<IQueryHandler<GetLastRegistrationForCurrentUserQuery, ContestRegistration>, GetLastRegistrationForCurrentUserQueryHandler>();
            services.AddTransient<IQueryHandler<ImportFromContestQuery, List<Contest>>, ImportFromContestQueryHandler>();
            services.AddTransient<IQueryHandler<SortingQuery, SortingQueryResult>, SortingQueryHandler>();
            services.AddTransient<IQueryHandler<GetDataForSortingQuery, DataForSorting>, GetDataForSortingQueryHandler>();
            services.AddTransient<IQueryHandler<GetContestRegistrationForEditQuery, ContestRegistrationViewModel>, GetContestRegistrationForEditQueryHandler>();
            services.AddTransient<IQueryHandler<GetContestRegistrationForCreateQuery, ContestRegistrationViewModel>, GetContestRegistrationForCreateQueryHandler>();
            services.AddTransient<IQueryHandler<GetContestDetailsQuery, ContestInfoViewModelBase>, GetContestDetailsQueryHandler>();

            services.AddTransient<ICommandHandler<ImportParticipantsCommand>, ImportParticipantsCommandHandler>(); 
            services.AddTransient<ICommandHandler<ImportFromContestCommand>, ImportFromContestCommandHandler>();
            services.AddTransient<ICommandHandler<SortingCommand>, SortingCommandHandler>();
            services.AddTransient<ICommandHandler<CancelRegistrationCommand>, CancelRegistrationCommandHandler>(); 
            services.AddTransient<ICommandHandler<DeleteRegistrationCommand>, DeleteRegistrationCommandHandler>();
        }
    }
}
