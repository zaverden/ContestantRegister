using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContestantRegister.Models
{
    public class ContestArea : DomainObject
    {
        public int ContestId { get; set; }
        public int AreaId { get; set; }

        public Contest Contest { get; set; }
        public Area Area { get; set; }        

        public List<ContestRegistration> ContestRegistrations { get; set; }
    }
}
