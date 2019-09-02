using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Features.Frontend.Home.Commands
{
    public class SendEmailCommand : ICommand
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
    }
}
