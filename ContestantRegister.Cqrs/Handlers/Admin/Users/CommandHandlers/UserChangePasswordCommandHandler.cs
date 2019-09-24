using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Admin.Users.CommandHandlers
{
    public class UserChangePasswordCommandHandler : ICommandHandler<UserChangePasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task HandleAsync(UserChangePasswordCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.Id);
            if (user == null)
                throw new EntityNotFoundException();

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, command.Password);
        }
    }
}
