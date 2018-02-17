using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels
{
    public class SuggectSchoolViewModel : SuggectStudyPlaceViewModel
    {
        [Display(Name = "Официальный email")]
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress]
        public string SchoolEmail { get; set; }

        
    }
}
