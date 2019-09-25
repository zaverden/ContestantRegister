using System.ComponentModel.DataAnnotations;
using ContestantRegister.Domain.Properties;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels
{
    public class SuggectSchoolViewModel : SuggectStudyPlaceViewModel
    {
        [Display(Name = "Официальный email")]
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmailErrorMessage")]
        public string SchoolEmail { get; set; }
        
    }
}
