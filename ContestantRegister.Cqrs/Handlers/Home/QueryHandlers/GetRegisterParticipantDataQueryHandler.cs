using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Cqrs.Features.Shared.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.QueryHandlers
{
    public class GetRegisterParticipantDataQueryHandler : ReadRepositoryQueryHandler<GetRegisterParticipantDataQuery, RegisterParticipantData>
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
