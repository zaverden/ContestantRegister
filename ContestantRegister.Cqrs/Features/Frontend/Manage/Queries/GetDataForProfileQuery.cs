using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Queries
{
    public class DataForProfile
    {
        public List<City> Cities { get; set; }
        public List<StudyPlaceDropdownItemViewModel> StudyPlaces { get; set; }
    }

    public class GetDataForProfileQuery : IQuery<DataForProfile>
    {
    }
}
