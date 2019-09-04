using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.Commands
{
    public class ConfirmEmailCommand : ICommand
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}
