using System.Collections.Generic;
using ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Queries
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
