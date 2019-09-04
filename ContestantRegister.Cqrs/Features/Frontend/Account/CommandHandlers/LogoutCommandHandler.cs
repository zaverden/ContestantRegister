using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Account.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.CommandHandlers
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(SignInManager<ApplicationUser> signInManager, ILogger<LogoutCommandHandler> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task HandleAsync(LogoutCommand command)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }
    }
}
