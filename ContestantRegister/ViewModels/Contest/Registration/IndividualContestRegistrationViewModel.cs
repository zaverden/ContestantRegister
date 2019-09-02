﻿using System.ComponentModel.DataAnnotations;
using ContestantRegister.Models;
using ContestantRegister.Properties;
using ContestantRegister.Services.ApplicationServices.Reg;

namespace ContestantRegister.ViewModels.Contest.Registration
{
    public class EditIndividualContestRegistrationViewModel : IndividualContestRegistrationViewModel, IEditIndividualContestRegistration
    {
        [Display(Name = "Участник")]
        public string ParticipantName { get; set; }
    }

    public class CreateIndividualContestRegistrationViewModel : IndividualContestRegistrationViewModel, ICreateIndividualContestRegistration
    {

    }

    public abstract class IndividualContestRegistrationViewModel : ContestRegistrationViewModel, IIndividualContestRegistration
    {
        [Display(Name = "Курс")]
        [Range(1, 6)]
        public int? Course { get; set; }

        [Display(Name = "Класс")]
        [Range(1, 11)]
        public int? Class { get; set; }

        [Display(Name = "Категория")]
        public StudentType? StudentType { get; set; }
    }

}
