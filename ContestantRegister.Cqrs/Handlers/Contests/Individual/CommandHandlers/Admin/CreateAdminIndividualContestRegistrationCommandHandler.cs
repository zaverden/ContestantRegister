using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices.ContestRegistration;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.CommandHandlers
{
    internal class CreateAdminIndividualContestRegistrationCommandHandler : CreateIndividualContestRegistrationCommandHandler<CreateAdminIndividualContestRegistrationCommand>
    {
        public CreateAdminIndividualContestRegistrationCommandHandler(
            IRepository repository, 
            IEmailSender emailSender, 
            ICurrentUserService currentUserService, 
            UserManager<ApplicationUser> userManager,
            IContestRegistrationService contestRegistrationService,
            IMapper mapper) 
            : base(repository, contestRegistrationService, emailSender, currentUserService, userManager, mapper)
        {
        
        }
        
    }
}
