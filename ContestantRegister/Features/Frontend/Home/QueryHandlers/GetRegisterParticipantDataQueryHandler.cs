using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Account.Queries;
using ContestantRegister.Data;
using ContestantRegister.Domain;
using ContestantRegister.Features.Frontend.Home.Queries;
using ContestantRegister.Features.Frontend.Home.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Features.Frontend.Home.QueryHandlers
{
    public class GetRegisterParticipantDataQueryHandler : ContextQueryHandler<GetRegisterParticipantDataQuery, RegisterParticipantData>
    {
        public GetRegisterParticipantDataQueryHandler(IReadRepository repository) : base(repository)
        {
            
        }
        public override async Task<RegisterParticipantData> HandleAsync(GetRegisterParticipantDataQuery query)
        {
            var cities = await ReadRepository.Set<City>()
                .OrderBy(x => x.Name)
                .ToListAsync();

            var studyPlaces = await ReadRepository.Set<StudyPlace>()
                .ProjectTo<StudyPlaceDropdownItemViewModel>()
                .OrderBy(x => x.ShortName)
                .ToListAsync();

            return new RegisterParticipantData
            {
                Cities = cities,
                StudyPlaces = studyPlaces,
            };
        }
    }
}
