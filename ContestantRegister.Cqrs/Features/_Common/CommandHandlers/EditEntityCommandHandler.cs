using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Models;
using ContestantRegister.Utils.Exceptions;

namespace ContestantRegister.Controllers._Common.CommandHandlers
{
    public class EditEntityCommandHandler<TEntity> : ContextCommandBaseHandler<EditEntityCommand<TEntity>> where TEntity : DomainObject
    {
        private readonly IMapper _mapper;

        public EditEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }

        public override async Task HandleAsync(EditEntityCommand<TEntity> command)
        {
            //TODO id можно отдельно передавать в команде, тогда ограничение "where TEntity : DomainObject" можно убрать
            var dbEntity = await Repository.FindAsync<TEntity>(command.Entity.Id);
            if (dbEntity == null) throw new EntityNotFoundException();
            
            _mapper.Map(command.Entity, dbEntity);

            await Repository.SaveChangesAsync();
        }
    }
}
