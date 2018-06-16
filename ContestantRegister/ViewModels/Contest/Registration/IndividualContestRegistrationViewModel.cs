using System.ComponentModel.DataAnnotations;
using ContestantRegister.Models;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels.Contest.Registration
{
    public class EditIndividualContestRegistrationViewModel : IndividualContestRegistrationViewModel
    {
        [Display(Name = "Участник")]
        public string ParticipantName { get; set; }
    }

    public class CreateIndividualContestRegistrationViewModel : IndividualContestRegistrationViewModel
    {

    }

    public abstract class IndividualContestRegistrationViewModel : ContestRegistrationViewModel
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
