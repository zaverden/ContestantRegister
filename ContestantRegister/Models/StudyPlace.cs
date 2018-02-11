using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public abstract class StudyPlace : DomainObject
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Город")]
        public int CityId { get; set; }

        public City City { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        [Display(Name = "Краткое название")]
        public string ShortName { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        [Display(Name = "Полное название")]
        public string FullName { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        [Display(Name = "Сайт")]
        [DataType(DataType.Url, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidUrlErrorMessage")]
        public string Site { get; set; }

        public ICollection<ContestRegistration> ContestRegistrations { get; set; }

        public ICollection<ContestantUser> Users { get; set; }
    }
}
