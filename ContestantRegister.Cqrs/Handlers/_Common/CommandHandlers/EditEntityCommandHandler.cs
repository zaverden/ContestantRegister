using System;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Services.Exceptions;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    public class EditEntityCommandHandler<TEntity, TKey> : RepositoryCommandBaseHandler<EditEntityCommand<TEntity>> 
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly IMapper Mapper;

        public EditEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository)
        {
            Mapper = mapper;
        }

        public override async Task HandleAsync(EditEntityCommand<TEntity> command)
        {
            //TODO id можно отдельно передавать в команде, тогда ограничение "where TEntity : DomainObject" можно убрать
            var dbEntity = await Repository.FindAsync<TEntity>(command.Entity.Id);
            if (dbEntity == null) throw new EntityNotFoundException();
            
            Mapper.Map(command.Entity, dbEntity);

            await Repository.SaveChangesAsync();
        }
    }
}
