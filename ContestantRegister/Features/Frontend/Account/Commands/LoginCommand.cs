using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers.Account.Commands
{
    public class LoginCommand : ICommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    
}
