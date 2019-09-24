using System.Collections.Generic;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Cqrs.Features.Shared.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries
{
    public class DataForContestRegistration
    {
        public int TrainerCount { get; set; }
        public bool IsAreaRequired { get; set; }

        public List<City> Cities { get; set; }

        public List<StudyPlaceDropdownItemViewModel> StudyPlaces { get; set; }
        public List<ContestArea> ContestAreas { get; set; }
        public List<UserForContestRegistrationListItemViewModel> Users { get; set; }
    }

    public class GetDataForContestRegistrationQuery : IQuery<DataForContestRegistration>
    {
        public int ContestId { get; set; }
    }
}
