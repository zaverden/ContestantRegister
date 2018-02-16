using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
