using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
