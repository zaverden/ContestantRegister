using System.Collections.Generic;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.CommandHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Cqrs.Features.Admin.Users.Queries;
using ContestantRegister.Cqrs.Features.Admin.Users.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
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
