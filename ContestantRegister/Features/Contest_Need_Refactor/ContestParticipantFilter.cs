using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Infrastructure.Filter.Attributes;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using ContestantRegister.Utils.Filter;

namespace ContestantRegister.Controllers
{
    public class ContestParticipantFilter
    {
        [RelatedObject("Participant1", PropertyName = "Surname")]
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string ParticipantName { get; set; }

        [RelatedObject("Trainer", PropertyName = "Surname")]
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string TrainerName { get; set; }

        [RelatedObject("Manager", PropertyName = "Surname")]
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string ManagerName { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        [RelatedObject("StudyPlace.City", PropertyName = "Name")]
        public string City { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        [RelatedObject("ContestArea.Area", PropertyName = "Name")]
        public string Area { get; set; }

        [ConvertFilter(typeof(EnumDisplayToValueConverter<ContestRegistrationStatus>))]
        public string Status { get; set; }

        [RelatedObject("StudyPlace", PropertyName = "ShortName")]
        [StringFilter(StringFilter.StartsWith, IgnoreCase = true)]
        public string StudyPlace { get; set; }
    }
}
