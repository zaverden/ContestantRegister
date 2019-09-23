using ContestantRegister.Cqrs.Features._Common.ListViewModel;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Framework.Filter;
using ContestantRegister.Framework.Filter.Attributes;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Queries
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

    public class GetContestDetailsQuery : IQuery<ContestInfoViewModelBase>
    {
        public ContestParticipantFilter Filter { get; set; }

        public int ContestId { get; set; }
    }
}
