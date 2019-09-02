using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Domain;

namespace ContestantRegister.Controllers._Common.CommandHandlers
{
    public class DeleteEntityCommandHandler<TEntity> : RepositoryCommandBaseHandler<DeleteEntityByIdCommand<TEntity>> where TEntity : class
    {
        public DeleteEntityCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override async Task HandleAsync(DeleteEntityByIdCommand<TEntity> command)
        {
            var entity = await Repository.FindAsync<TEntity>(command.Id);
            Repository.Remove(entity);
            await Repository.SaveChangesAsync();
        }
    }
}
