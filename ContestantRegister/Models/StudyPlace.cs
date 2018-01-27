using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public abstract class StudyPlace : DomainObject
    {
        [Required]
        public City City { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShortName { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Site { get; set; }

        public ICollection<ContestRegistration> ContestRegistrations { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
    }
}
