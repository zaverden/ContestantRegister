using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class City : DomainObject
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<StudyPlace> StudyPlaces { get; set; }
    }
}
