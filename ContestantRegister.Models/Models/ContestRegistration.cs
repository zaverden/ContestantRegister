﻿using System;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Domain.Properties;

namespace ContestantRegister.Models
{
    public abstract class ContestRegistration : DomainObject
    {
        [Display(Name = "Участник")]
        public string Participant1Id { get; set; }

        public ApplicationUser Participant1 { get; set; }
        
        [Display(Name = "Дата регистрации")]
        public DateTime? RegistrationDateTime { get; set; }

        [Display(Name = "Кем зарегистрирован")]
        public ApplicationUser RegistredBy { get; set; }

        [Display(Name = "Статус регистрации")]
        public ContestRegistrationStatus Status { get; set; }
        
        [Display(Name = "Тренер")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string TrainerId { get; set; }

        public ApplicationUser Trainer { get; set; }

        [Display(Name = "Руководитель")]
        public string ManagerId { get; set; }

        public ApplicationUser Manager { get; set; }

        [Display(Name = "Учебное заведение")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int StudyPlaceId { get; set; }

        public StudyPlace StudyPlace { get; set; }

        [Display(Name = "Контест")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int ContestId { get; set; }

        public Contest Contest { get; set; }

        [Display(Name = "Рабочее место")]
        [MaxLength(50)]
        public string ComputerName { get; set; }

        [Display(Name = "Язык программирования")]
        [MaxLength(100)]
        public string ProgrammingLanguage { get; set; }

        [Display(Name = "Логин в ЯКонтесте")]
        [MaxLength(50)]
        public string YaContestLogin { get; set; }

        [Display(Name = "Пароль в ЯКонтесте")]
        [MaxLength(50)]
        public string YaContestPassword { get; set; }

        [Display(Name = "Площадка")]
        public int? ContestAreaId { get; set; }

        public ContestArea ContestArea { get; set; }

        [Display(Name = "№")]
        [Range(1, int.MaxValue)]
        public int Number { get; set; }

        [Display(Name = "Вне конкурса")]
        public bool IsOutOfCompetition { get; set; }
    }
}
