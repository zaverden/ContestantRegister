using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Users.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.CommandHandlers
{
    public class UserRemoveRoleCommandHandler : ICommandHandler<UserRemoveRoleCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRemoveRoleCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task HandleAsync(UserRemoveRoleCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.Id);

            await _userManager.RemoveFromRoleAsync(user, command.Role);
        }
    }
}
