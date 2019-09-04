using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.QueryHandlers
{
    public class GetUserForDetailsQueryHandler : ReadRepositoryQueryHandler<GetUserForDetailsQuery, ApplicationUser>
    {
        public GetUserForDetailsQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<ApplicationUser> HandleAsync(GetUserForDetailsQuery query)
        {
            return await  ReadRepository.Set<ApplicationUser>()
                .Include(u => u.StudyPlace.City)
                .SingleOrDefaultAsync(m => m.Id == query.Id);
        }
    }
}
