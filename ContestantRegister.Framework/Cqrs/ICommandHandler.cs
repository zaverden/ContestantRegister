using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    //TODO можно добавить CancellationToken для асинхронных методов Handle, будет архитектурненько, но не нужно :)
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}
