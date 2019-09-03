using ContestantRegister.Models;

namespace ContestantRegister.ViewModels.ListItemViewModels
{
    public class UserListItemViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public UserType UserType { get; set; }
    }
}
