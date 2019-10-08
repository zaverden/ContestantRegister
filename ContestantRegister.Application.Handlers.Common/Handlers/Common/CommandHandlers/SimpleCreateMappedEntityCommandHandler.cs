using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain.Repository;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    internal class SimpleCreateMappedEntityCommandHandler<TEntity> : CreateMappedEntityCommandHandler<CreateMappedEntityCommand<TEntity, TEntity>, TEntity, TEntity>
        where TEntity : new()
    {
        public SimpleCreateMappedEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
