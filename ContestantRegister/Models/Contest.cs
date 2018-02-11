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
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Начало регистрации")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime RegistrationStart { get; set; }

        [Display(Name = "Регистрация до")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationEnd { get; set; }

        //TODO сделать ли дробным? чтобы контест был по 3,5 часа
        [Display(Name = "Продолжительность (ч)")]
        [Range(1, int.MaxValue)]
        public int Duration { get; set; }

        [Display(Name = "Участники")]
        public ParticipantType ParticipantType { get; set; }

        [Display(Name = "Первенство")]
        public ContestType ContestType { get; set; }

        [Display(Name = "Архивный")]
        public bool IsArchive { get; set; }

        [Display(Name = "Статус")]
        public ContestStatus ContestStatus { get; set; }

        [Display(Name = "Участие")]
        public ContestParticipationType ContestParticipationType { get; set; }

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

        [Display(Name = "CSV с яконтест аккаунтами")]
        [DataType(DataType.MultilineText)]
        public string YaContestAccountsCSV { get; set; }

        [Display(Name = "Использовано яконтест аккаунтов")]
        [Range(0, int.MaxValue)]
        public int UsedAccountsCount { get; set; } = 0;

        public ICollection<ContestRegistration> ContestRegistrations { get; set; }

        //TODO площадка - список площадок и спрашивать ли площадку
    }
}
