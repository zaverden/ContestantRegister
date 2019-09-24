using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Home.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Services.InfrastructureServices;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.CommansHandlers
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
