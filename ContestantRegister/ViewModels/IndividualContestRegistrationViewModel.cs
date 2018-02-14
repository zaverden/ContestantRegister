using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Models;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels
{
    public class IndividualContestRegistrationViewModel
    {
        [Display(Name = "Участник")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string Participant1Id { get; set; }

        public ContestantUser Participant1 { get; set; }

        [Display(Name = "Тренер")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string TrainerId { get; set; }

        public ContestantUser Trainer { get; set; }

        [Display(Name = "Руководитель")]
        public string ManagerId { get; set; }

        public ContestantUser Manager { get; set; }

        [Display(Name = "Место учебы")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int StudyPlaceId { get; set; }

        public int ContestId { get; set; }

        //TODO можно заменить просто названеим контеста
        public Contest Contest { get; set; }

        [Display(Name = "Язык программирования")]
        [MaxLength(100)]
        public string ProgrammingLanguage { get; set; }

        [Display(Name = "Город")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int CityId { get; set; }
    }
}
