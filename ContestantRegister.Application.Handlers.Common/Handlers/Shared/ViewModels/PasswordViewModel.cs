using System.ComponentModel.DataAnnotations;
using ContestantRegister.Domain.Properties;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels
{
    public class PasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PasswordLengthErrorMessage")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
