using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Commands
{
    public class UserChangePasswordCommand : ICommand
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }
}
