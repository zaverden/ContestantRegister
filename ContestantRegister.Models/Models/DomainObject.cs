using ContestantRegister.Domain;

namespace ContestantRegister.Models
{
    public abstract class DomainObject : IHasId<int>
    {
        public int Id { get; set; }
    }
}
