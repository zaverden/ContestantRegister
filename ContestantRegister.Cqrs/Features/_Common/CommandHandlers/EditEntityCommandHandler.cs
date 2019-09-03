using System;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Models;
using ContestantRegister.Utils.Exceptions;

namespace ContestantRegister.Controllers._Common.CommandHandlers
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
