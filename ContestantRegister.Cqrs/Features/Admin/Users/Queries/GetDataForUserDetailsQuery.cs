using System.Collections.Generic;
using ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.Queries
{
    public class DataForUserDetails
    {
        public List<City> Cities { get; set; }
        public List<StudyPlaceDropdownItemViewModel> StudyPlaces { get; set; }
    }

    public class GetDataForUserDetailsQuery : IQuery<DataForUserDetails>
    {
    }
}
