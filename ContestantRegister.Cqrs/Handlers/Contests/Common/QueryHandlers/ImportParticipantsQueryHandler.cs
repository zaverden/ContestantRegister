using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.QueryHandlers
{
    public class ImportParticipantsQueryHandler : ReadRepositoryQueryHandler<ImportParticipantsQuery, ImportParticipantsViewModel>

    {
        public ImportParticipantsQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<ImportParticipantsViewModel> HandleAsync(ImportParticipantsQuery query)
        {
            var contestName = await ReadRepository.Set<Contest>()
                .Where(x => x.Id == query.ContestId)
                .Select(x => x.Name)
                .SingleOrDefaultAsync();
            if (contestName == null)
            {
                throw new EntityNotFoundException();
            }

            var vm = new ImportParticipantsViewModel
            {
                ContestName = contestName
            };

            return vm;
        }
    }
}
