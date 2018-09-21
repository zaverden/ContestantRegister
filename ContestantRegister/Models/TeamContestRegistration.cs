using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public class TeamContestRegistration : ContestRegistration
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Участник 2")]
        public string Participant2Id { get; set; }

        public ApplicationUser Participant2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Участник 3")]
        public string Participant3Id { get; set; }

        public ApplicationUser Participant3 { get; set; }

        /// <summary>
        /// Например, Bizons
        /// </summary>
        [Display(Name = "Название команды")]
        [MaxLength(128)]
        public string TeamName { get; set; }

        /// <summary>
        /// ISIT SFU 1
        /// </summary>
        [Display(Name = "Официальное название команды")]
        [MaxLength(128)]
        public string OfficialTeamName { get; set; }


        public string DisplayTeamName
        {
            get
            {
                return
                    string.IsNullOrEmpty(TeamName) ?
                    OfficialTeamName :
                    $"{OfficialTeamName}: {TeamName}";
            }
        }

    }
}
