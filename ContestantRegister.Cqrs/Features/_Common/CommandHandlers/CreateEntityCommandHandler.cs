using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Domain;

namespace ContestantRegister.Controllers._Common.CommandHandlers
{
    public class CreateEntityCommandHandler<TEntity> : ContextCommandBaseHandler<CreateEntityCommand<TEntity>>
    {
        public CreateEntityCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override async Task HandleAsync(CreateEntityCommand<TEntity> command)
        {
            Repository.Add(command.Entity);

            await Repository.SaveChangesAsync();            
        }
    }
}
