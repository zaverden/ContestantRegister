using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Queries;
using ContestantRegister.Cqrs.Features.Shared.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.QueryHandlers
{
    public class GetDataForContestRegistrationQueryHandler : ReadRepositoryQueryHandler<GetDataForContestRegistrationQuery, DataForContestRegistration>
    {
        public GetDataForContestRegistrationQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<DataForContestRegistration> HandleAsync(GetDataForContestRegistrationQuery query)
        {
            var result = new DataForContestRegistration();
            // для командного контеста загружаются регистрации. зачем - непонятно, пока сделал без них

            result.Cities = ReadRepository.Set<City>().OrderBy(x => x.Name).ToList();

            result.StudyPlaces = ReadRepository.Set<StudyPlace>()
                .ProjectTo<StudyPlaceDropdownItemViewModel>()
                .OrderBy(x => x.ShortName)
                .ToList();

            var contest = await ReadRepository.Set<Contest>()
                .Where(x => x.Id == query.ContestId)
                .Select(x => new
                {
                    x.TrainerCount,
                    x.IsAreaRequired
                })
                .SingleOrDefaultAsync();

            result.TrainerCount = contest.TrainerCount;
            result.IsAreaRequired = contest.IsAreaRequired;

            if (contest.IsAreaRequired)
            {
                result.ContestAreas = await ReadRepository.Set<ContestArea>()
                    .Include(x => x.Area)
                    .OrderBy(x => x.Area.Name)
                    .Where(x => x.ContestId == query.ContestId)
                    .ToListAsync();
            }

            result.Users = await ReadRepository.Set<ApplicationUser>()
                .ProjectTo<UserForContestRegistrationListItemViewModel>()
                .OrderBy(x => x.DisplayName)
                .ToListAsync();
            
            return result;
        }
    }
}
