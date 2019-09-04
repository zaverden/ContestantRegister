using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common.Commands
{
    public class DeleteEntityByIdCommand<TEntity, TKey> : ICommand
    {
        public TKey Id { get; set; }
    }
}
