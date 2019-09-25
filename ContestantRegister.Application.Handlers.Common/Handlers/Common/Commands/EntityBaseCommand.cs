using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features._Common.Commands
{
    public abstract class EntityBaseCommand<TEntity> : ICommand
    {
        public TEntity Entity { get; set; }
    }
}
