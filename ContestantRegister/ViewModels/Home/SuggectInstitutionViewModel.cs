using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels.HomeViewModels
{
    public class SuggectInstitutionViewModel : SuggectStudyPlaceViewModel
    {
        [Display(Name = "Краткое название на английском языке")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string ShortNameEn { get; set; }

        [Display(Name = "Полное название на английском языке")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string FullNameEn { get; set; }
    }
}
