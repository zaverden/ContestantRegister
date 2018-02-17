using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels
{
    public class SuggectStudyPlaceViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress]
        [Display(Name = "Email для уведомления о создании учебного заведения")]
        public string Email { get; set; }

        [Display(Name = "Краткое название")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string ShortName { get; set; }

        [Display(Name = "Полное название")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string FullName { get; set; }

        [Display(Name = "Город")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string City { get; set; }

        [Display(Name = "Сайт")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [DataType(DataType.Url)]
        public string Site { get; set; }

        public bool IsEmailReadonly { get; set; }
    }
}
