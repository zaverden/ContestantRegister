using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Controllers._Common.Queries
{
    //TODO для красоты можно передавать Id в конструктор, но тогда нельзя будет создавать генерики, там допустим только конструктор без параметров
    //TODO а плодить какие-то фабрики не хочется 
    public abstract class EntityIdBaseQuery<T> : IQuery<T>
    {
        public int Id { get; set; }
    }
}
