using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public class CompClass : DomainObject
    {
        [Display(Name = "Название")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "Количество машин")]
        [Range(1, 30)]
        public int CompNumber { get; set; }

        [Display(Name = "Площадка")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        public string Area { get; set; }

        [Display(Name = "Комментарий")]
        [MaxLength(500)]
        public string Comment { get; set; }

        public ICollection<ContestCompClass> ContestCompClasses { get; set; }
    }
}
