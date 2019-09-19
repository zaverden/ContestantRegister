using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    public abstract class CommandHandlerMiddleware : ICommandHandler<ICommand>
    {
        protected readonly object Next;

        protected CommandHandlerMiddleware(object next)
        {
            //можно проверить, что реализует ICommandHandler
            Next = next;
        }

        public abstract Task HandleAsync(ICommand command);
    }
}
