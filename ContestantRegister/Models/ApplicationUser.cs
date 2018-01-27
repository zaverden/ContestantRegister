using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegistrationDateTime { get; set; }

        public ApplicationUser RegistredBy { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; }

        [Required]
        [MaxLength(50)]
        public string Patronymic { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public StudyPlace StudyPlace { get; set; }

        public ICollection<ContestRegistration> ContestRegistrationsRegistredBy { get; set; }
        public ICollection<ContestRegistration> ContestRegistrationsParticipant1 { get; set; }
        public ICollection<ContestRegistration> ContestRegistrationsTrainer { get; set; }
        public ICollection<ContestRegistration> ContestRegistrationsManager { get; set; }

        public ICollection<TeamContestRegistration> ContestRegistrationsParticipant2 { get; set; }
        public ICollection<TeamContestRegistration> ContestRegistrationsParticipant3 { get; set; }

        public ICollection<ApplicationUser> RegistredByThisUser { get; set; }

    }
}
