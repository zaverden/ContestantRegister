using System.Threading.Tasks;

namespace ContestantRegister.Framework.Cqrs
{
    public abstract class CommandHandlerMiddleware : ICommandHandler<ICommand>
    {
        private readonly object _next;

        // параметр типа object, так как это может быть как ICommandHandler<ICommand>, так и ICommandHandler<T>, будет конфликт генериков.
        // его можно избежать, сделав базовый интерфейс типа ICommandHandler без генерик-параметров, но я не хочу. для middleware отсутствие типизации для перехвата любого запроса - это нормально
        protected CommandHandlerMiddleware(object next)
        {
            //можно проверить, что реализует ICommandHandler
            _next = next;
        }

        protected Task HandleNextAsync(ICommand command)
        {
            var handlemethod = _next.GetType().GetMethod("HandleAsync");
            var res = handlemethod.Invoke(_next, new object[] { command });
            return (Task)res;
        }

        public abstract Task HandleAsync(ICommand command);
    }
}
