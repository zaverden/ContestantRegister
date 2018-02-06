using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    public class TeamContestRegistration : ContestRegistration
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Участник 2")]
        public ContestantUser Participant2 { get; set; }

        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Участник 3")]
        public ContestantUser Participant3 { get; set; }
    }
}
