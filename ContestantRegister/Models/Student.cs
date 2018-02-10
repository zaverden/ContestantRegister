using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    [DisplayName("Студент")]
    public class Student : ContestantUser
    {
        [Display(Name = "Дата начала обучения")]
        public DateTime EducationStartDate { get; set; }

        [Display(Name = "Дата завершения обучения")]
        public DateTime EducationEndDate { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime DateOfBirth { get; set; }
    }
}
