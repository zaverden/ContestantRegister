using System;

namespace ContestantRegister.Domain
{
    public interface IHasId<TKey>
        where  TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}
