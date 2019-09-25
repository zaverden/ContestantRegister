using System.Collections.Generic;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features._Common.Queries
{
    public class GetMappedEntitiesQuery<TEntity, TViewModel> : IQuery<List<TViewModel>>
    {
    }
}
