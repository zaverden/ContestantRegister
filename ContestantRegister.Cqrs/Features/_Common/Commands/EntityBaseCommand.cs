using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common.Commands
{
    public abstract class EntityBaseCommand<TEntity> : ICommand
    {
        public TEntity Entity { get; set; }
    }
}
