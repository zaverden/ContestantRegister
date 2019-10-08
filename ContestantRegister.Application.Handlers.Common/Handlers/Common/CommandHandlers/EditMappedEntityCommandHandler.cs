using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Services.Exceptions;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    internal class EditMappedEntityCommandHandler<TEntity, TViewModel, TKey> : RepositoryCommandBaseHandler<EditMappedEntityCommand<TEntity, TViewModel, TKey>> 
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly IMapper Mapper;

        public EditMappedEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository)
        {
            Mapper = mapper;
        }


        public override async Task HandleAsync(EditMappedEntityCommand<TEntity, TViewModel, TKey> command)
        {
            var validationResult = await ValidateViewModel(command.Entity);
            if (validationResult?.Count > 0)
                throw new ValidationException(validationResult);

            var dbEntity = await Repository.FindAsync<TEntity>(command.Id);
            if (dbEntity == null) throw new EntityNotFoundException();

            Mapper.Map(command.Entity, dbEntity);

            await Repository.SaveChangesAsync();
        }

        protected virtual Task<List<KeyValuePair<string, string>>> ValidateViewModel(TViewModel viewModel)
        {
            return Task.FromResult<List<KeyValuePair<string, string>>>(null);
        }
    }
}
