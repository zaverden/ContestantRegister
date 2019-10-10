using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Services.Exceptions;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    internal class CreateMappedEntityCommandHandler<TCommand, TEntity, TViewModel> : RepositoryCommandBaseHandler<TCommand> 
        where TEntity : new()
        where TCommand : CreateMappedEntityCommand<TEntity, TViewModel>
    {
        protected readonly IMapper Mapper;
        public CreateMappedEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository)
        {
            Mapper = mapper;
        }

        public override async Task HandleAsync(TCommand command)
        {
            var validationResult = await ValidateViewModel(command.Entity);
            if (validationResult?.Count > 0)
                throw new ValidationException(validationResult);

            var newEntity = new TEntity();

            await InitNewEntity(newEntity, command);

            Mapper.Map(command.Entity, newEntity);

            Repository.Add(newEntity);

            await Repository.SaveChangesAsync();
        }

        protected virtual Task<List<KeyValuePair<string, string>>> ValidateViewModel(TViewModel viewModel)
        {
            return Task.FromResult<List<KeyValuePair<string, string>>>(null);
        }

        protected virtual Task InitNewEntity(TEntity entity, TCommand command)
        {
            return Task.FromResult(0);
        }
    }
}
