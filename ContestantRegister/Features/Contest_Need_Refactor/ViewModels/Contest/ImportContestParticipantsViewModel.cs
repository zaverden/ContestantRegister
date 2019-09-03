using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels.Contest
{
    public class ImportContestParticipantsViewModel
    {
        [Display(Name = "Логины участников")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string ParticipantYaContestLogins { get; set; }

        [Display(Name = "Контест для импорта")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int FromContestId { get; set; }
    }
}
