using System;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.CommandHandlers
{
    public class ChangePasswordCommandHandler : RepositoryCommandBaseHandler<ChangePasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(
            IRepository repository, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ILogger<ChangePasswordCommandHandler> logger,
            ICurrentUserService currentUserService) : base(repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public override async Task HandleAsync(ChangePasswordCommand command)
        {
            var user = await _userManager.FindByEmailAsync(_currentUserService.Email);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with email '{_currentUserService.Email}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
            if (!changePasswordResult.Succeeded)
                throw new UnableToChangePasswordException(changePasswordResult.Errors);

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
        }
    }
}
