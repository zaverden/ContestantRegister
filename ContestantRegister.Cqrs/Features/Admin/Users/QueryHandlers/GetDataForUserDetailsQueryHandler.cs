using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Users.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.QueryHandlers
{
    public class GetDataForUserDetailsQueryHandler : ReadRepositoryQueryHandler<GetDataForUserDetailsQuery, DataForUserDetails>
    {
        public GetDataForUserDetailsQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<DataForUserDetails> HandleAsync(GetDataForUserDetailsQuery query)
        {
            var result = new DataForUserDetails();

            result.Cities = await ReadRepository.Set<City>()
                .OrderBy(x => x.Name)
                .ToListAsync();

            result.StudyPlaces = await ReadRepository.Set<StudyPlace>()
                .ProjectTo<StudyPlaceDropdownItemViewModel>()
                .ToListAsync();

            return result;
        }
    }
}
