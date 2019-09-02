using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Controllers.Account.Queries;
using ContestantRegister.Data;
using ContestantRegister.Domain;
using ContestantRegister.Features.Frontend.Account.ViewModels;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.ListItemViewModels;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Controllers.Account.QueryHandlers
{
    public class GetDataForRegistrationQueryHandler : ReadRepositoryQueryHandler<GetDataForRegistrationQuery, DataForRegistration>
    {
        public GetDataForRegistrationQueryHandler(IReadRepository repository) : base(repository)
        {
        }
        public override async Task<DataForRegistration> HandleAsync(GetDataForRegistrationQuery query)
        {
            var result = new DataForRegistration();

            result.Cities = await ReadRepository.Set<City>()
                .OrderBy(c => c.Name)
                .ToListAsync();

            result.StudyPlaces = await ReadRepository.Set<StudyPlace>()
                .ProjectTo<StudyPlaceDropdownItemViewModel>()
                .OrderBy(x => x.ShortName)
                .ToListAsync();

            return result;
        }
    }
}
