using System.Threading.Tasks;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Features.Frontend.Account.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Features.Frontend.Account.CommandHandlers
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
