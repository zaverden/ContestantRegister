using ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.Commands
{
    public class LoginCommand : ICommand
    {
        public LoginViewModel ViewModel { get; set; }
    }

    
}
