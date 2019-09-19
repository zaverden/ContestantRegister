using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Services.DomainServices;
using ContestantRegister.Services.Exceptions;

namespace ContestantRegister.Cqrs.Features.Admin.Users.CommandHandlers
{
    public class ValidateUserCommandHandlerDecorator<TCommand, TEntity> : CommandHandlerDecorator<TCommand>
        where TCommand : EntityBaseCommand<TEntity>
        where TEntity : IApplicationUser
    {
        private readonly IUserService _userService;

        public ValidateUserCommandHandlerDecorator(ICommandHandler<TCommand> next, IUserService userService) : base(next)
        {
            _userService = userService;
        }

        public override async Task HandleAsync(TCommand command)
        {
            var validationResult = await _userService.ValidateUserAsync(command.Entity);
            if (validationResult.Count > 0)
                throw new ValidationException(validationResult);

            await Next.HandleAsync(command);
        }
    }
}
