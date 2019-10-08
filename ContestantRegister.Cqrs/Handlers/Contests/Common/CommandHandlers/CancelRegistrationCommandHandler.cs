using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers
{
    internal class CancelRegistrationCommandHandler : RepositoryCommandBaseHandler<CancelRegistrationCommand>
    {
        public CancelRegistrationCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override async Task HandleAsync(CancelRegistrationCommand command)
        {
            var registration = await Repository.Set<ContestRegistration>().SingleOrDefaultAsync(r => r.Id == command.RegistrationId);
            if (registration == null) throw new EntityNotFoundException();
            
            registration.Status = ContestRegistrationStatus.NotCompleted;

            await Repository.SaveChangesAsync();

            //TODO переделать навигацию, чтобы на бэк отправлялся не только id регистрации, но и контесте, тогжа этого хака не нужно будет. хак нужен из-за серверной навигации
            command.ContestId = registration.ContestId;
        }
    }
}
