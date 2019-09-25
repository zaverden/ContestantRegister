using System.Collections.Generic;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features._Common.Queries
{
    public class GetEntitiesQuery<TEntity> : IQuery<List<TEntity>>
    {

    }
}
