using System.Threading.Tasks;
using ContestantRegister.Features.Frontend.Account.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using ContestantRegister.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContestantRegister.Features.Frontend.Account.CommandHandlers
{
    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper _urlHelper;

        public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
        }

        public async Task HandleAsync(ForgotPasswordCommand command)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null)
                throw new EntityNotFoundException();

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _urlHelper.Action(command.Action, command.Controller, new []{user.Id, code }, command.Scheme);
            await _emailSender.SendEmailAsync(command.Email, "Сброс пароля на сайте олимпиад ИКИТ СФУ",
                $"Для сброса пароля нажмите на ссылку: <a href='{callbackUrl}'>ссылка</a>");
        }
    }
}
