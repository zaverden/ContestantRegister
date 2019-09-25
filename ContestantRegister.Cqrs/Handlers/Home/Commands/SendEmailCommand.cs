using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Commands
{
    public class SendEmailCommand : ICommand
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
    }
}
