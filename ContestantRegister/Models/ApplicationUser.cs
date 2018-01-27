using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDateTime { get; set; }

        [Display(Name = "Кем зарегистрирован")]
        public ApplicationUser RegistredBy { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        /// <summary>
        /// Поле необязательно для школьника итренера школьников, но обязаьельно для студента и тренера студентов
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Поле необязательно для школьника итренера школьников, но обязаьельно для студента и тренера студентов
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Место учебы")]
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
