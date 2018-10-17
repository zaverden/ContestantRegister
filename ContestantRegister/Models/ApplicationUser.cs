using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Роль")]
        public UserType UserType { get; set; }

        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDateTime { get; set; }

        [Display(Name = "Кем зарегистрирован")]
        public ApplicationUser RegistredBy { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
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

        /// <summary>
        /// Это поле пустое у дефолтового админа 
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Учебное заведение")]
        public int StudyPlaceId { get; set; }

        public StudyPlace StudyPlace { get; set; }

        [Display(Name = "Дата начала обучения")]
        public DateTime? EducationStartDate { get; set; }

        [Display(Name = "Дата завершения обучения")]
        public DateTime? EducationEndDate { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime? DateOfBirth { get; set; }

        //обязательное для студента 
        [Display(Name = "Категория")]
        public StudentType? StudentType { get; set; }

        /// <summary>
        /// Когда студент не завершил регистрацию, но нужно срочно ее завершить, заводим аналогичного с фейковым email 
        /// Поле видно только для админа
        /// </summary>
        [Display(Name = "Baylor email")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        public string BaylorEmail { get; set; }

        [Display(Name = "Регистрация на бейлоре завершена")]
        public bool IsBaylorRegistrationCompleted { get; set; }

        [Display(Name = "Ссылка на профиль вконтакте")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        [DataType(DataType.Url, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidUrlErrorMessage")]
        public string VkProfile { get; set; }

        public ICollection<ContestRegistration> ContestRegistrationsRegistredBy { get; set; }
        public ICollection<ContestRegistration> ContestRegistrationsParticipant1 { get; set; }
        public ICollection<ContestRegistration> ContestRegistrationsTrainer { get; set; }
        public ICollection<ContestRegistration> ContestRegistrationsManager { get; set; }

        public ICollection<TeamContestRegistration> ContestRegistrationsParticipant2 { get; set; }
        public ICollection<TeamContestRegistration> ContestRegistrationsParticipant3 { get; set; }
        public ICollection<TeamContestRegistration> ContestRegistrationsReserveParticipant { get; set; }

        public ICollection<ApplicationUser> RegistredByThisUser { get; set; }
    }
}
