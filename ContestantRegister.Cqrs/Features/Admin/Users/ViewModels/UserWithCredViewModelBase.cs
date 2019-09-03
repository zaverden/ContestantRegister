
using ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels;

namespace ContestantRegister.ViewModels
{
    public abstract class UserWithCredViewModelBase : UserViewModelBase
    {
        //TODO PasswordViewModel теперь в Account,  запилить новую 
        public PasswordViewModel PasswordViewModel { get; set; }
    }
}
