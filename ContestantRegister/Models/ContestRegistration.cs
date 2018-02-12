﻿using System;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public abstract class ContestRegistration : DomainObject
    {
        [Display(Name = "Data регистрации")]
        public DateTime? RegistrationDateTime { get; set; }

        [Display(Name = "Кем зарегистрирован")]
        public ContestantUser RegistredBy { get; set; }

        [Display(Name = "Статус регистрации")]
        public ContestRegistrationStatus Status { get; set; }

        [Display(Name = "Участник")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int Participant1Id { get; set; }

        public ContestantUser Participant1 { get; set; }

        [Display(Name = "Тренер")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int TrainerId { get; set; }

        public ContestantUser Trainer { get; set; }

        [Display(Name = "Руководитель")]
        public int ManagerId { get; set; }

        public ContestantUser Manager { get; set; }

        [Display(Name = "Место учебы")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int StudyPlaceId { get; set; }

        public StudyPlace StudyPlace { get; set; }

        [Display(Name = "Контест")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int ContestId { get; set; }

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
