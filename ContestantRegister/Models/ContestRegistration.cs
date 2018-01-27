using System;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public abstract class ContestRegistration : DomainObject
    {
        public DateTime RegistrationDateTime { get; set; }

        [Required]
        public ApplicationUser RegistredBy { get; set; }

        [Required]
        public ApplicationUser Participant1 { get; set; }

        [Required]
        public ApplicationUser Trainer { get; set; }

        public ApplicationUser Manager { get; set; }

        [Required]
        public StudyPlace StudyPlace { get; set; }

        [Required]
        public Contest Contest { get; set; }

        [MaxLength(100)]
        public string ProgrammingLanguage { get; set; }

        /// <summary>
        /// Комп класс и номер компа, выданные при регистрации
        /// </summary>
        [MaxLength(20)]
        public string ComputerName { get; set; }

        [MaxLength(20)]
        public string YaContestLogin { get; set; }

        [MaxLength(20)]
        public string YaContestPassword { get; set; }

        //TODO площадка

    }
}
