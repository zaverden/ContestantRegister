using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Features.Frontend.Home.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Features.Frontend.Home.Queries
{
    public class RegisterParticipantData
    {
        public List<StudyPlaceDropdownItemViewModel> StudyPlaces { get; set; }
        public List<City> Cities { get; set; }

    }
    public class GetRegisterParticipantDataQuery : IQuery<RegisterParticipantData>
    {

    }
}
