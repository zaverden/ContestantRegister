
using ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels;

namespace ContestantRegister.Cqrs.Features.Admin.Users.ViewModels
{
    public abstract class UserWithCredViewModelBase : UserViewModelBase
    {
        //TODO PasswordViewModel теперь в Account,  запилить новую 
        public PasswordViewModel PasswordViewModel { get; set; }
    }
}
