using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class ContestCompClassParticipant : DomainObject
    {
        [Range(1, 30)]
        public int CompNumber { get; set; }

        public string ParticipantId { get; set; }
        public ApplicationUser Participant { get; set; }

        public int ContestCompClassId { get; set; }
        public ContestCompClass ContestCompClass { get; set; }
    }
}
