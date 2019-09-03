namespace ContestantRegister.Controllers._Common.Queries
{
    public class GetEntityByIdForDeleteQuery<TEntity, TKey> : EntityIdBaseQuery<TEntity, TKey>
    {
        public string[] IncludeProperties { get; set; }
    }
}
