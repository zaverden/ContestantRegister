using System;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public abstract class ContestRegistration : DomainObject
    {
        [Display(Name = "Data регистрации")]
        public DateTime? RegistrationDateTime { get; set; }

        [Display(Name = "Кем зарегистрирован")]
        public ApplicationUser RegistredBy { get; set; }

        [Display(Name = "Статус регистрации")]
        public ContestRegistrationStatus Status { get; set; }
        
        [Display(Name = "Участник")]
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public ApplicationUser Participant1 { get; set; }

        [Display(Name = "Тренер")]
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public ApplicationUser Trainer { get; set; }

        [Display(Name = "Руководитель")]
        public ApplicationUser Manager { get; set; }

        [Display(Name = "Место учебы")]
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public StudyPlace StudyPlace { get; set; }

        [Display(Name = "Контест")]
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public Contest Contest { get; set; }

        [Display(Name = "Язык программирования")]
        [MaxLength(100)]
        public string ProgrammingLanguage { get; set; }

        [Display(Name = "Рабочее место")]
        [MaxLength(20)]
        public string ComputerName { get; set; }

        [Display(Name = "Логин в ЯКонтесте")]
        [MaxLength(20)]
        public string YaContestLogin { get; set; }

        [Display(Name = "Пароль в ЯКонтесте")]
        [MaxLength(20)]
        public string YaContestPassword { get; set; }

        //TODO площадка

    }
}
