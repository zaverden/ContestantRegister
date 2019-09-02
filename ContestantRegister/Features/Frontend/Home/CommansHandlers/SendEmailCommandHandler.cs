using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Features.Frontend.Home.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Services;

namespace ContestantRegister.Features.Frontend.Home.CommansHandlers
{
    public class SendEmailCommandHandler : ICommandHandler<SendEmailCommand>
    {
        private readonly IEmailSender _emailSender;

        public SendEmailCommandHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task HandleAsync(SendEmailCommand command)
        {
            await _emailSender.SendEmailAsync(command.Email, command.Subject, command.Body);
        }
    }
}
