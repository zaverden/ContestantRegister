using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Commands
{
    public class UserRemoveRoleCommand : ICommand
    {
        public string Role { get; set; }

        public string Id { get; set; }
    }
}
