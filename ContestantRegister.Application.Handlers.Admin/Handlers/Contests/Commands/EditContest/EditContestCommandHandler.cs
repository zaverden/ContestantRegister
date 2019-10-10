using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Contests.Utils;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.CommandHandlers
{
    internal class EditContestCommandHandler : EditMappedEntityCommandHandler<Contest, ContestDetailsViewModel, int>
    {
        public EditContestCommandHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public override async Task HandleAsync(EditMappedEntityCommand<Contest, ContestDetailsViewModel, int> command)
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
