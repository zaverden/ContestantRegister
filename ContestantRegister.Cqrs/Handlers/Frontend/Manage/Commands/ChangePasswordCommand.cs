using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Commands
{
    public class ChangePasswordCommand : ICommand
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
