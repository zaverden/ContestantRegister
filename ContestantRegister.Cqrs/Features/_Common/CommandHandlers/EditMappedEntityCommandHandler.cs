using System;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Utils.Exceptions;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    public class EditMappedEntityCommandHandler<TEntity, TViewModel, TKey> : RepositoryCommandBaseHandler<EditMappedEntityCommand<TEntity, TViewModel, TKey>> 
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly IMapper Mapper;

        public EditMappedEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository)
        {
            Mapper = mapper;
        }


        public override async Task HandleAsync(EditMappedEntityCommand<TEntity, TViewModel, TKey> command)
        {
            var dbEntity = await Repository.FindAsync<TEntity>(command.Id);
            if (dbEntity == null) throw new EntityNotFoundException();

            Mapper.Map(command.Entity, dbEntity);

            await Repository.SaveChangesAsync();
        }
    }
}
