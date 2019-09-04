using ContestantRegister.Domain.Properties;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels
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
