using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain.Repository;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    internal class CreateEntityCommandHandler<TEntity> : RepositoryCommandBaseHandler<CreateEntityCommand<TEntity>>
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
