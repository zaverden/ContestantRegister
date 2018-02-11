using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    [DisplayName("Контест")]
    public class Contest : DomainObject
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(200)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [MaxLength(5000)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Начало регистрации")]
        public DateTime RegistrationStart { get; set; }

        [Display(Name = "Регистрация до")]
        public DateTime RegistrationEnd { get; set; }

        [Display(Name = "Продолжительность (ч)")]
        public int Duration { get; set; }

        [Display(Name = "Участники")]
        public ParticipantType ParticipantType { get; set; }

        [Display(Name = "Первенство")]
        public ContestType ContestType { get; set; }

        [Display(Name = "Архивный")]
        public bool IsArchive { get; set; }

        [Display(Name = "Статус")]
        public ContestStatus ContestStatus { get; set; }

        [Display(Name = "Английский язык")]
        public bool IsEnglishLanguage { get; set; }

        [Display(Name = "Указывать язык программирования при регистрации")]
        public bool IsProgrammingLanguageNeeded { get; set; }

        [MaxLength(100)]
        [Display(Name = "Ссылка на контест на ЯКонтесте")]
        [DataType(DataType.Url, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidUrlErrorMessage")]
        public string YaContestLink { get; set; }

        [Display(Name = "Отправлять email с регистрационными данными после завершения регистрации")]
        public bool SendRegistrationEmail { get; set; }

        [Display(Name = "Показывать на сайте логин/пароль от ЯКонтест")]
        public bool ShowRegistrationInfo { get; set; }

        public string YaContestAccountsCSV { get; set; }

        public int UsedAccountsCount { get; set; }

        public ICollection<ContestRegistration> ContestRegistrations { get; set; }

        //TODO площадка - список площадок и спрашивать ли площадку
    }
}
