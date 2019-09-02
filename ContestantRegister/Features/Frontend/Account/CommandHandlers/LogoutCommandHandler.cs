using System.Threading.Tasks;
using ContestantRegister.Features.Frontend.Account.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ContestantRegister.Features.Frontend.Account.CommandHandlers
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
