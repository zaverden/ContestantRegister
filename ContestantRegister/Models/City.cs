using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    [DisplayName("Город")]
    public class City : DomainObject
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Регион")]
        //[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int? RegionId { get; set; }

        public Region Region { get; set; }

        public ICollection<StudyPlace> StudyPlaces { get; set; }
    }
}
