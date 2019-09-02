using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common.Commands
{
    public class DeleteEntityByIdCommand<TEntity> : ICommand
    {
        public int Id { get; set; }
    }
}
