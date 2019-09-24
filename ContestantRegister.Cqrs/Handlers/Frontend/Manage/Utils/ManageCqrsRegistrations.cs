using ContestantRegister.Cqrs.Features.Frontend.Manage.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Manage.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Framework.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Utils
{
    public static class ManageCqrsRegistrations
    {
        public static void RegisterManageServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetUserDetailsQuery, IndexViewModel>, GetUserDetailsQueryHandler>();
            services.AddTransient<IQueryHandler<GetDataForProfileQuery, DataForProfile>, GetDataForProfileQueryHandler>();

            services.AddTransient<ICommandHandler<ChangePasswordCommand>, ChangePasswordCommandHandler>();
            services.AddTransient<ICommandHandler<SaveUserCommand>, SaveUserCommandHandler>();
        }
    }
}
