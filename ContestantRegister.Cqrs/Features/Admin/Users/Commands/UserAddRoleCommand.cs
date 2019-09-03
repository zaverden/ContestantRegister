using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.Commands
{
    public class UserAddRoleCommand : ICommand
    {
        public string Role { get; set; }

        public string Id { get; set; }
    }
}
