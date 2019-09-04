using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    // при сильном желании можно сделать по отдельному диспетчеру для команд и запросов
    // но пока нет физического разделения чтения и записи (разные базы) в этом нет смысла. будут разные базы - можно будет поднять отдельную инфраструктуру только для query для чтения из нее
    // а пока проще команды и запросы хранить вместе в одной фиче. если что - разделить их будет несложно ) 
    public interface IHandlerDispatcher
    {
        Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query);

        Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand;

    }
}
