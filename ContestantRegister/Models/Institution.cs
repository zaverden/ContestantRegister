using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public class Institution : StudyPlace
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Short name")]
        public string ShortNameEnglish { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(200)]
        [Display(Name = "Full Name")]
        public string FullNameEnglish { get; set; }
        
        [MaxLength(200)]
        [Display(Name = "Ссылка на страницу на бейлоре")]
        public string BaylorLink { get; set; }
    }
}
