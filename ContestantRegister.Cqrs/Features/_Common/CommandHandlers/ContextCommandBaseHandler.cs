using System.Threading.Tasks;
using ContestantRegister.Domain;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common.CommandHandlers
{
    public abstract class ContextCommandBaseHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        protected readonly IRepository Repository;

        protected ContextCommandBaseHandler(IRepository repository)
        {
            Repository = repository;
        }

        public abstract Task HandleAsync(TCommand command);
    }
}
