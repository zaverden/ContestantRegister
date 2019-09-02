using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class Area : DomainObject
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}
