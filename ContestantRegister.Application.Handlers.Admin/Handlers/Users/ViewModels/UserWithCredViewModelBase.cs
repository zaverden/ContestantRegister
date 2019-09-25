
namespace ContestantRegister.Cqrs.Features.Admin.Users.ViewModels
{
    public abstract class UserWithCredViewModelBase : UserViewModelBase
    {
        //TODO PasswordViewModel теперь в Account,  запилить новую 
        public Frontend.Account.ViewModels.PasswordViewModel PasswordViewModel { get; set; }
    }
}
