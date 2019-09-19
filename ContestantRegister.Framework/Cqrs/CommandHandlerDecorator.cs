using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    public abstract class CommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        protected readonly ICommandHandler<TCommand> Next;
        
        protected CommandHandlerDecorator(ICommandHandler<TCommand> next)
        {
            Next = next;
        }
        
        public abstract Task HandleAsync(TCommand command);
    }
}
