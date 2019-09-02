using System.Threading.Tasks;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Features.Frontend.Account.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Features.Frontend.Account.CommandHandlers
{
    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task HandleAsync(ResetPasswordCommand command)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null)
                throw new EntityNotFoundException();

            var code = command.Code.Replace(" ", "+");
            var result = await _userManager.ResetPasswordAsync(user, code, command.Password);
            if (!result.Succeeded)
                throw new UnableToResetPasswordException(result.Errors);

            // Если пользователь был зареган преподом, у него может быть не подтвержден пароль. 
            // Если это так, то при восстановлении пароля нужно подтвердить email? иначе пользователь потом не сможет войти в новым паролем, так как email не подтвержден
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new UnableToConfirmEmailException(result.Errors);
            }
        }
    }
}
