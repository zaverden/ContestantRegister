using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Commands
{
    public class CreateUserCommand : CreateMappedEntityCommand<ApplicationUser, CreateUserViewModel>
    {
        
    }
}
