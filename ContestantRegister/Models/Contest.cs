using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class Contest : DomainObject
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(5000)]
        public string Description { get; set; }

        public DateTime RegistrationStart { get; set; }

        public DateTime RegistrationEnd { get; set; }

        public int Duration { get; set; }

        public ParticipantType ParticipantType { get; set; }

        public ContestType ContestType { get; set; }

        public bool IsArchive { get; set; }

        public ContestStatus ContestStatus { get; set; }

        public bool IsEnglishLanguage { get; set; }

        /// <summary>
        /// Справшивать ли язык программирования при регистрации
        /// </summary>
        public bool IsProgrammingLanguageNeeded { get; set; }

        [MaxLength(100)]
        public string YaContestLink { get; set; }

        /// <summary>
        /// Отправлять ли email  с регистрационными данными (ссылка на яконтест, логин, пароль) на email после регистрации?
        /// </summary>
        public bool SendRegistrationEmail { get; set; }

        /// <summary>
        /// Показывать ли в системе логин и пароль сразу после регистрациии
        /// </summary>
        public bool ShowRegistrationInfo { get; set; }

        public string YaContestAccountsCSV { get; set; }

        public int UsedAccountsCount { get; set; }

        public ICollection<ContestRegistration> ContestRegistrations { get; set; }

        //TODO площадка - список площадок и спрашивать ли площадку
    }
}
