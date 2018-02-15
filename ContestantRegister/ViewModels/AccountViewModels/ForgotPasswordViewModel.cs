using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
