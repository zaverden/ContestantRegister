using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class ContestCompClass : DomainObject
    {
        public int ContestId { get; set; }
        public int CompClassId { get; set; }

        public Contest Contest { get; set; }
        public CompClass CompClass { get; set; }
    }
}
