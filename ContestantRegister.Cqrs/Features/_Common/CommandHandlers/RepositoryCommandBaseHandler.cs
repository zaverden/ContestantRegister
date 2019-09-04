using System.Threading.Tasks;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    public abstract class RepositoryCommandBaseHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        protected readonly IRepository Repository;

        protected RepositoryCommandBaseHandler(IRepository repository)
        {
            Repository = repository;
        }

        public abstract Task HandleAsync(TCommand command);
    }
}
