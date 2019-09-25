using System.Collections.Generic;
using ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Queries
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
