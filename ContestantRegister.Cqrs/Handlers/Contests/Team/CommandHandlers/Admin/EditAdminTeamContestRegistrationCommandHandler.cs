﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices.ContestRegistration;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.CommandHandlers
{
    internal class EditAdminTeamContestRegistrationCommandHandler : EditTeamContestRegistrationCommandHandler<EditAdminTeamContestRegistrationCommand>
    {
        public EditAdminTeamContestRegistrationCommandHandler(
            IRepository repository, 
            IContestRegistrationService contestRegistrationService, 
            IMapper mapper, 
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager) : 
            base(repository, contestRegistrationService, mapper, currentUserService, userManager)
        {
            
        }
    }
}
