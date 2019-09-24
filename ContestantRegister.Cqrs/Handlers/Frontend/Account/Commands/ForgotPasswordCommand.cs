using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.Commands
{
    public class ForgotPasswordCommand : ICommand
    {
        
        public string Scheme { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Email { get; set; }
    }
}
