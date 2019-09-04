using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.CommandHandlers;
using ContestantRegister.Domain;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Commands
{
    public class ChangePasswordCommandHandler : RepositoryCommandBaseHandler<ChangePasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        public ChangePasswordCommandHandler(IRepository repository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<ChangePasswordCommandHandler> logger) : base(repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public override async Task HandleAsync(ChangePasswordCommand command)
        {
            var user = await _userManager.FindByEmailAsync(command.CurrentUserEmail);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with email '{command.CurrentUserEmail}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
            if (!changePasswordResult.Succeeded)
                throw new UnableToChangePasswordException(changePasswordResult.Errors);

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
        }
    }
}
