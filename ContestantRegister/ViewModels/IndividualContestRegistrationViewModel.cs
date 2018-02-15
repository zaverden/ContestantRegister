using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Models;
using ContestantRegister.Properties;
using Microsoft.AspNetCore.Mvc;

namespace ContestantRegister.ViewModels
{
    public class IndividualContestRegistrationViewModel
    {
        [Display(Name = "Участник")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Remote(action: "VerifyIndividualContestParticipant", controller: "Home", AdditionalFields = nameof(ContestId))]
        public string Participant1Id { get; set; }

        public int ContestId { get; set; }

        [Display(Name = "Тренер")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string TrainerId { get; set; }

        [Display(Name = "Руководитель")]
        public string ManagerId { get; set; }

        [Display(Name = "Место учебы")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int StudyPlaceId { get; set; }

        public string ContestName { get; set; }

        [Display(Name = "Язык программирования")]
        [MaxLength(100)]
        public string ProgrammingLanguage { get; set; }

        [Display(Name = "Город")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int CityId { get; set; }
    }
}
