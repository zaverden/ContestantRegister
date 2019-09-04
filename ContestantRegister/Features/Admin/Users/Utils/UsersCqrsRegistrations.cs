using System.Collections.Generic;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Schools;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Users.CommandHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Users;
using ContestantRegister.Cqrs.Features.Frontend.Users.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Users.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Users.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Users.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.UserViewModels;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace ContestantRegister.Features.Admin.Schools.Utils
{
    public static class UsersCqrsRegistrations
    {
        public static void RegisterUsersServices(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetUsersQuery, List<UserListItemViewModel>>, GetEntitiesQueryHandler<ApplicationUser, UserListItemViewModel>>();
            services.AddTransient<IQueryHandler<GetEntityByIdQuery<ApplicationUser, string>, ApplicationUser>, GetEntityQueryHandler<ApplicationUser, string>>();
            services.AddTransient<IQueryHandler<GetEntityByIdForDeleteQuery<ApplicationUser, string>, ApplicationUser>, GetEntityForDeleteQueryHandler<ApplicationUser, string>>();

            services.AddTransient<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();
            services.AddTransient<ICommandHandler<EditMappedEntityCommand<ApplicationUser, EditUserViewModel, string>>, EditUserCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteEntityByIdCommand<ApplicationUser, string>>, DeleteEntityCommandHandler<ApplicationUser, string>>();

            services.AddTransient<IQueryHandler<GetExportedUsersQuery, ExcelPackage>, GetExportedUsersQueryHandler>();
            services.AddTransient<IQueryHandler<GetDataForUserDetailsQuery, DataForUserDetails>, GetDataForUserDetailsQueryHandler>();
            services.AddTransient<IQueryHandler<GetAdminsQuery, List<UserAdminViewModel>>, GetAdminsQueryHandler>();

            services.AddTransient<ICommandHandler<UserAddRoleCommand>, UserAddRoleCommandHandler>();
            services.AddTransient<ICommandHandler<UserRemoveRoleCommand>, UserRemoveRoleCommandHandler>();
            services.AddTransient<ICommandHandler<UserChangePasswordCommand>, UserChangePasswordCommandHandler>();
        }
    }
}
