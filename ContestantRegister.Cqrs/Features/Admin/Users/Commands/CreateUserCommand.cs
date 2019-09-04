using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Cqrs.Features._Common.Commands;
using ContestantRegister.Models;
using ContestantRegister.ViewModels.UserViewModels;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Commands
{
    public class CreateUserCommand : CreateMappedEntityCommand<ApplicationUser, CreateUserViewModel>
    {
        public string CurrentUserEmail { get; set; }
    }
}
