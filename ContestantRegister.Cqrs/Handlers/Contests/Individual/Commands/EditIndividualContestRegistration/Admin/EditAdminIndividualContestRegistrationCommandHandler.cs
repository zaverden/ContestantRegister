using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices.ContestRegistration;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.CommandHandlers
{
    internal class EditAdminIndividualContestRegistrationCommandHandler : EditIndividualContestRegistrationCommandHandler<EditAdminIndividualContestRegistrationCommand>
    {
        public EditAdminIndividualContestRegistrationCommandHandler(
            IRepository repository, 
            IContestRegistrationService contestRegistrationService, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService
            ) 
            : base(repository, contestRegistrationService, mapper, userManager, currentUserService)
        {
            
        }

        
    }
}
