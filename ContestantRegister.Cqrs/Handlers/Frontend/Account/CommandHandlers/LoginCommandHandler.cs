using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Account.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.CommandHandlers
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        public LoginCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<LoginCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;

        }
        public async Task HandleAsync(LoginCommand command)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null) throw new EntityNotFoundException();

            if (!await _userManager.IsEmailConfirmedAsync(user)) throw new EmailNotConfirmedException();

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(command.Email, command.Password, command.RememberMe, lockoutOnFailure: false);

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                throw new UserLockedOutException();
            }

            if (!result.Succeeded) throw new InvalidLoginAttemptException();

            _logger.LogInformation("User logged in.");
        }
    }
}
