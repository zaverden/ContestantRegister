using System.Collections.Generic;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common.Queries
{
    public class GetEntitiesWithMappingQuery<TEntity, TViewModel> : IQuery<List<TViewModel>>
    {
    }
}
