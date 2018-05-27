using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class ContestArea : DomainObject
    {
        public int ContestId { get; set; }
        public int AreaId { get; set; }

        public Contest Contest { get; set; }
        public Area Area { get; set; }

        [Display(Name = "Комп. классы")]
        [MaxLength(1000)]
        [ReadOnly(true)]
        [DataType(DataType.MultilineText)]
        public string SortingResults { get; set; }

        [MaxLength(200)]
        public string SortingCompClassIds { get; set; }

        public List<ContestRegistration> ContestRegistrations { get; set; }
    }
}
