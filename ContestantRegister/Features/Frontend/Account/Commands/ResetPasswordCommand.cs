﻿using ContestantRegister.Infrastructure.Cqrs;

namespace ContestantRegister.Features.Frontend.Account.Commands
{
    public class ResetPasswordCommand : ICommand
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
    }
}
