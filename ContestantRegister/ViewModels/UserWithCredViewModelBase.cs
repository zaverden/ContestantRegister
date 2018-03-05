using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels
{
    public abstract class UserWithCredViewModelBase : UserViewModelBase
    {
        public PasswordViewModel PasswordViewModel { get; set; }
    }
}
