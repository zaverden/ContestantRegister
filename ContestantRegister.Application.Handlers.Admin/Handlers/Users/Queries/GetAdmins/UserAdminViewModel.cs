using System.ComponentModel.DataAnnotations;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Users.ViewModels
{
    public class UserAdminViewModel
    {
        [Display(Name = "Пользователь")]
        public ApplicationUser User { get; set; }

        [Display(Name = "Админ")]
        public bool IsAdmin { get; set; }
    }
}
