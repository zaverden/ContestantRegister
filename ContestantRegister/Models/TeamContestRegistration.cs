using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class TeamContestRegistration : ContestRegistration
    {
        [Required]
        public ApplicationUser Participant2 { get; set; }

        [Required]
        public ApplicationUser Participant3 { get; set; }
    }
}
