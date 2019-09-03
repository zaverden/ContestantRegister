using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    public class CreateMappedEntityCommandHandler<TEntity, TViewModel> : RepositoryCommandBaseHandler<CreateMappedEntityCommand<TEntity, TViewModel>> where TEntity : new()
    {
        protected readonly IMapper Mapper;
        public CreateMappedEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository)
        {
            Mapper = mapper;
        }

        public override async Task HandleAsync(CreateMappedEntityCommand<TEntity, TViewModel> command)
        {
            var newEntity = new TEntity();

            Mapper.Map(command.Entity, newEntity);

            Repository.Add(newEntity);

            await Repository.SaveChangesAsync();
        }
    }
}
