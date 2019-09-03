using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Models;
using ContestantRegister.Utils.Exceptions;

namespace ContestantRegister.Cqrs.Features._Common.CommandHandlers
{
    public class EditMappedEntityCommandHandler<TEntity, TViewModel> : RepositoryCommandBaseHandler<EditMappedEntityCommand<TEntity, TViewModel>> where TEntity : DomainObject
    {
        protected readonly IMapper Mapper;

        public EditMappedEntityCommandHandler(IRepository repository, IMapper mapper) : base(repository)
        {
            Mapper = mapper;
        }


        public override async Task HandleAsync(EditMappedEntityCommand<TEntity, TViewModel> command)
        {
            var dbEntity = await Repository.FindAsync<TEntity>(command.Id);
            if (dbEntity == null) throw new EntityNotFoundException();

            Mapper.Map(command.Entity, dbEntity);

            await Repository.SaveChangesAsync();
        }
    }
}
