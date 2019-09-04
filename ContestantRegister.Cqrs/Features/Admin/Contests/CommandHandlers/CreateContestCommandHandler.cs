using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Contests.Utils;
using ContestantRegister.Cqrs.Features.Admin.Contests.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Contests.CommandHandlers
{
    public class CreateContestCommandHandler : CreateMappedEntityCommandHandler<CreateMappedEntityCommand<Contest, ContestDetailsViewModel>, Contest, ContestDetailsViewModel>
    {
        public CreateContestCommandHandler(IRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public override async Task HandleAsync(CreateMappedEntityCommand<Contest, ContestDetailsViewModel> command)
        {
            //Если создавать контест на винде, перевод строки там два символа. А потом при регистрации на никсах идет сплит по переводу строки, а это один символ. И \r добавляется сзади к паролю.
            //Это ломает экспорт в csv и при отправле пароля по email этот символ рендерится как пробел
            command.Entity.RemoveWindowsLineEnds();

            var contest = new Contest();

            await SaveContestHelper.SyncManyToMany(command.Entity.SelectedAreaIds, Repository.Set<Area>(), command.Entity.ContestAreas, SaveContestHelper.CreateContestAreaRelation, SaveContestHelper.CmpArea, contest);

            Mapper.Map(command.Entity, contest);

            Repository.Add(contest);

            await Repository.SaveChangesAsync();
        }
        
    }
}
