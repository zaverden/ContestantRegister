﻿using System.ComponentModel.DataAnnotations;

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
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        [Required]
        public bool IsSended { get; set; }
    }
}
