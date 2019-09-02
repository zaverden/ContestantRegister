using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels
{
    public abstract class UserWithCredViewModelBase : UserViewModelBase
    {
        //TODO PasswordViewModel теперь в Account,  запилить новую 
        public PasswordViewModel PasswordViewModel { get; set; }
    }
}
