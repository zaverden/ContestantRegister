using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Account.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.CommandHandlers
{
    public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task HandleAsync(ConfirmEmailCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
                throw new EntityNotFoundException($"Unable to load user with ID '{command.UserId}'.");
            
            var code = command.Code.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
                throw new UnableToConfirmEmailException(result.Errors);
        }
    }
}
