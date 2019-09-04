using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Commands
{
    public class SaveUserCommand : ICommand
    {
        public IndexViewModel ViewModel { get; set; }

        public string CurrentUserEmail { get; set; }
    }
}
