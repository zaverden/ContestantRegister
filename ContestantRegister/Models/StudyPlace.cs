using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public abstract class StudyPlace : DomainObject
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Город")]
        public City City { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Краткое название")]
        public string ShortName { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(200)]
        [Display(Name = "Полное название")]
        public string FullName { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(200)]
        [Display(Name = "Сайт")]
        public string Site { get; set; }

        public ICollection<ContestRegistration> ContestRegistrations { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
    }
}
