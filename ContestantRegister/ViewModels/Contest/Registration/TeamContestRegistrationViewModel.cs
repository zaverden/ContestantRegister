using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels.Contest.Registration
{
    public class CreateTeamContestRegistrationViewModel : TeamContestRegistrationViewModel
    {
        
    }

    public class EditTeamContestRegistrationViewModel : TeamContestRegistrationViewModel
    {

    }

    public abstract class TeamContestRegistrationViewModel : ContestRegistrationViewModel
    {
        [Display(Name = "Участник 2")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string Participant2Id { get; set; }

        [Display(Name = "Участник 3")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string Participant3Id { get; set; }

        [Display(Name = "Запасной участник")]
        public string ReserveParticipantId { get; set; }

        [Display(Name = "Тренер 2")]
        public string Trainer2Id { get; set; }

        [Display(Name = "Тренер 3")]
        public string Trainer3Id { get; set; }

        [Display(Name = "Неофициальное название команды")]
        [MaxLength(128)]
        public string TeamName { get; set; }

        [Display(Name = "Официальное название команды")]
        [MaxLength(128)]
        public string OfficialTeamName { get; set; }
    }
}
