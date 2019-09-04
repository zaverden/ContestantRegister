using ContestantRegister.Cqrs.Features.Frontend.Account.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Account.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Account.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Account.QueryHandlers;
using ContestantRegister.Framework.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister.Features.Frontend.Account.Utils
{
    public static class AccountCqrsRegistrations
    {
        public static void RegisterAccountServices(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<LoginCommand>, LoginCommandHandler>();
            services.AddTransient<IQueryHandler<GetDataForRegistrationQuery, DataForRegistration>, GetDataForRegistrationQueryHandler>();
            services.AddTransient<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
            services.AddTransient<ICommandHandler<LogoutCommand>, LogoutCommandHandler>();
            services.AddTransient<ICommandHandler<ConfirmEmailCommand>, ConfirmEmailCommandHandler>();
            services.AddTransient<ICommandHandler<ForgotPasswordCommand>, ForgotPasswordCommandHandler>();
            services.AddTransient<ICommandHandler<ResetPasswordCommand>, ResetPasswordCommandHandler>();
        }
    }
}
