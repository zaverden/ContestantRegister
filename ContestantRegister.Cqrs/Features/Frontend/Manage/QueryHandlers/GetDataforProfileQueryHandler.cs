using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.QueryHandlers
{
    public class GetDataForProfileQueryHandler : ReadRepositoryQueryHandler<GetDataForProfileQuery, DataForProfile>
    {
        public GetDataForProfileQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<DataForProfile> HandleAsync(GetDataForProfileQuery query)
        {
            var result = new DataForProfile();

            result.Cities = await ReadRepository.Set<City>()
                .OrderBy(x => x.Name)
                .ToListAsync();

            result.StudyPlaces = await ReadRepository.Set<StudyPlace>()
                .ProjectTo<StudyPlaceDropdownItemViewModel>()
                .OrderBy(x => x.ShortName)
                .ToListAsync();

            return result;
        }
    }
}
