using System;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Domain.Repository;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    internal class DeleteEntityByIdCommandHandler<TEntity, TKey> : RepositoryCommandBaseHandler<DeleteEntityByIdCommand<TEntity, TKey>> 
        where TEntity : class, IHasId<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        public DeleteEntityByIdCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override async Task HandleAsync(DeleteEntityByIdCommand<TEntity, TKey> command)
        {
            Repository.RemoveById<TEntity, TKey>(command.Id);
            await Repository.SaveChangesAsync();
        }
    }
}
