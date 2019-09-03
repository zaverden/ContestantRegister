using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Framework.Extensions;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Utils.ViewModelsSorting;

namespace ContestantRegister.Controllers._Common.QueryHandlers
{
    public class GetEntitiesQueryHandler<TEntity, TViewModel> : ReadRepositoryQueryHandler<GetMappedEntitiesQuery<TEntity, TViewModel>, List<TViewModel>> where TEntity : class
    {
        public GetEntitiesQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<List<TViewModel>> HandleAsync(GetMappedEntitiesQuery<TEntity, TViewModel> query)
        {
            var items = ReadRepository.Set<TEntity>()
                .ProjectTo<TViewModel>()
                .AutoFilter(query);

            var orderBy = OrderByCache.GetOrderBy(typeof(TViewModel));

            if (!orderBy.IsEmpty())
            {
                items = items.OrderBy(orderBy);
            }
            
            var res = await items.ToListAsync();

            return res;
        }
    }
}
