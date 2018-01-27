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
        [MaxLength(50)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public ICollection<StudyPlace> StudyPlaces { get; set; }
    }
}
