﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.ViewModels.Contest;

namespace ContestantRegister.Features.Frontend.Home.Commands
{
    public class RegisterContestParticipantCommand : ICommand
    {
        public RegisterContestParticipantViewModel RegisterContestParticipantViewModel { get; set; }
        public string CurrentUserEmail { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Scheme { get; set; }
    }
}
