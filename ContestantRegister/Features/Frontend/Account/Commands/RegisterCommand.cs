using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.ViewModels.AccountViewModels;

namespace ContestantRegister.Controllers.Account.Commands
{
    public class RegisterCommand : ICommand
    {
        //TODO по хорошему вью-модели лучше убрать из команд, ну да ладно, потом отрефакторю, ибо нужно сделать большой DTO :)
        public RegisterViewModel RegisterViewModel { get; set; }
        public string RequestScheme { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }
}
