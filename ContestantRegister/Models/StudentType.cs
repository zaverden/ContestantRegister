using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Models
{
    public enum StudentType
    {
        [Display (Name = "Студент")]
        Student = 1,

        [Display(Name = "Магистрант")]
        Magister = 2,

        [Display(Name = "Аспирант")]
        Aspirant = 3
    }
}
