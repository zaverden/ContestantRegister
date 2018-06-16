using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels.Contest
{
    public class SuggectSchoolViewModel : SuggectStudyPlaceViewModel
    {
        [Display(Name = "Официальный email")]
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmailErrorMessage")]
        public string SchoolEmail { get; set; }
        
    }
}
