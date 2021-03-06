﻿using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features._Common.Queries
{
    //TODO для красоты можно передавать Id в конструктор, но тогда нельзя будет создавать генерики, там допустим только конструктор без параметров
    //TODO а плодить какие-то фабрики не хочется 
    public abstract class EntityIdBaseQuery<TEntity, TKey> : IQuery<TEntity>
    {
        public TKey Id { get; set; }
    }
}
