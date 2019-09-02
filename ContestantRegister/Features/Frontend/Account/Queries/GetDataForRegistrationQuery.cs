using ContestantRegister.Models;
using ContestantRegister.ViewModels.ListItemViewModels;
using System.Collections.Generic;
using ContestantRegister.Features.Frontend.Account.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers.Account.Queries
{
    public class DataForRegistration
    {
        public List<City> Cities { get; set; }

        public List<StudyPlaceDropdownItemViewModel> StudyPlaces { get; set; }
    }

    public class GetDataForRegistrationQuery : IQuery<DataForRegistration>
    {
    }
}
