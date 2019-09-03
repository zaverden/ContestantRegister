using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Controllers._Common.Commands;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Contests.Utils;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Models;
using ContestantRegister.Utils.Exceptions;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.CommandHandlers
{
    public class EditContestCommandHandler : EditMappedEntityCommandHandler<Contest, ContestDetailsViewModel>
    {
        public EditContestCommandHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public override async Task HandleAsync(EditMappedEntityCommand<Contest, ContestDetailsViewModel> command)
        {
            command.Entity.RemoveWindowsLineEnds();

            var dbContest = await Repository.Set<Contest>()
                .Include(x => x.ContestAreas)
                .SingleOrDefaultAsync(x => x.Id == command.Id);
            if (dbContest == null) throw new EntityNotFoundException();

            await SaveContestHelper.SyncManyToMany(command.Entity.SelectedAreaIds, Repository.Set<Area>(), dbContest.ContestAreas, SaveContestHelper.CreateContestAreaRelation, SaveContestHelper.CmpArea, dbContest);

            Mapper.Map(command.Entity, dbContest);

            Repository.Update(dbContest);
            await Repository.SaveChangesAsync();
        }
    }
}
