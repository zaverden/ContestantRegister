using System.ComponentModel.DataAnnotations;
using ContestantRegister.Domain.Properties;

namespace ContestantRegister.Cqrs.Features.Frontend.Account.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmailErrorMessage")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
