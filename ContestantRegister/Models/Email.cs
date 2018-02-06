using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Models
{
    public class Email : DomainObject
    {
        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        [StringLength(4000)]
        public string Message { get; set; }

        [Required]
        public bool IsSended { get; set; }
    }
}
