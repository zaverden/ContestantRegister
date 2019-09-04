using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.CommandHandlers
{
    public class ChangePasswordCommand : ICommand
    {
        public string CurrentUserEmail { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
