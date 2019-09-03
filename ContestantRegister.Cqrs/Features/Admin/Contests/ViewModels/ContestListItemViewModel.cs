using System;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels
{
    public class ContestListItemViewModel
    {
        [OrderBy(IsDesc = true)]
        public int Id { get; set; }

        [Display(Name = "Название")]

        public string Name { get; set; }

        [Display(Name = "Регистрация до")]
        public DateTime RegistrationEnd { get; set; }

        [Display(Name = "Старт")]
        public DateTime Start { get; set; }

        [Display(Name = "Часов")]
        public int Duration { get; set; }

        [Display(Name = "Участники")]
        public ParticipantType ParticipantType { get; set; }

        [Display(Name = "Первенство")]
        public ContestType ContestType { get; set; }

        [Display(Name = "Участие")]
        public ContestParticipationType ContestParticipationType { get; set; }

        [Display(Name = "Архивный")]
        public bool IsArchive { get; set; }

        [Display(Name = "Статус")]
        public ContestStatus ContestStatus { get; set; }

    }
}
