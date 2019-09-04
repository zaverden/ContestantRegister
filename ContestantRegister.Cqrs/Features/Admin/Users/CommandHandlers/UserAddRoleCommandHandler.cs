using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Admin.Users.CommandHandlers
{
    public class UserAddRoleCommandHandler : ICommandHandler<UserAddRoleCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAddRoleCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task HandleAsync(UserAddRoleCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.Id);

            await _userManager.AddToRoleAsync(user, command.Role);
        }
    }
}
