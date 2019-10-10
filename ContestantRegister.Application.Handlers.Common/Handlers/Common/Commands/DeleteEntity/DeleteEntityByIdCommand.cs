using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features._Common.Commands
{
    public class DeleteEntityByIdCommand<TEntity, TKey> : ICommand
    {
        public TKey Id { get; set; }
    }
}
