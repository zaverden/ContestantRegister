using System.ComponentModel.DataAnnotations;
using System.Text;
using ContestantRegister.Models;
using ContestantRegister.Properties;
using ContestantRegister.Services.ApplicationServices.Reg;

namespace ContestantRegister.ViewModels.Contest.Registration
{
    public class CreateTeamContestRegistrationViewModel : TeamContestRegistrationViewModel, ICreateTeamContestRegistration
    {
        
    }

    public class EditTeamContestRegistrationViewModel : TeamContestRegistrationViewModel, IEditTeamContestRegistration
    {

    }

    public abstract class TeamContestRegistrationViewModel : ContestRegistrationViewModel, ITeamContestRegistration
    {
        [Display(Name = "Участник 2")]
        public string Participant2Id { get; set; }

        [Display(Name = "Участник 3")]
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

        public override ContestRegistrationStatus CheckRegistrationStatus()
        {
            if (string.IsNullOrEmpty(Participant1Id) || string.IsNullOrEmpty(Participant2Id) || string.IsNullOrEmpty(Participant3Id))
                return ContestRegistrationStatus.NotCompleted;

            return ContestRegistrationStatus.Completed;
        }
    }
}
