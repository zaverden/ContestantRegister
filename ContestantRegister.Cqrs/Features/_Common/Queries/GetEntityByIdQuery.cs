namespace ContestantRegister.Cqrs.Features._Common.Queries
{
    public class GetEntityByIdQuery<TEntity, TKey> : EntityIdBaseQuery<TEntity, TKey>
    {
        //Можно сделать не массив строк, а массив Expression<Func<TEntity, object>>, но это не будет работать для сложных случаев а-ля ThenInclude, когда нужно сделать цепочку инклудов
        public string[] IncludeProperties { get; set; }
    }
}
