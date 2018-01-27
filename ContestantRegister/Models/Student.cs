using System;

namespace ContestantRegister.Models
{
    public class Student : ApplicationUser
    {
        public DateTime EducationStartDate { get; set; }

        public DateTime EducationEndDate { get; set; }

        public DateTime DateOfBirth { get; set; }


    }
}
