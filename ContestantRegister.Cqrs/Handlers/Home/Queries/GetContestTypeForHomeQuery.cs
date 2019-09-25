using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Queries
{
    public class GetContestTypeForHomeQuery : GetEntityByIdQuery<ContestType, int>
    {
    }
}
