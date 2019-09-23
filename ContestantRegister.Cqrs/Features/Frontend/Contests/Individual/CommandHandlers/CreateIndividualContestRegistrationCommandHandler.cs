using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices.ContestRegistration;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.CommandHandlers
{
    public class CreateIndividualContestRegistrationCommandHandler : CreateContestRegistrationCommandHandler<CreateIndividualContestRegistrationCommand>
    {
        private readonly IContestRegistrationService _contestRegistrationService;
        private readonly IMapper _mapper;

        public CreateIndividualContestRegistrationCommandHandler(
            IRepository repository, 
            IEmailSender emailSender, 
            ICurrentUserService currentUserService, 
            UserManager<ApplicationUser> userManager,
            IContestRegistrationService contestRegistrationService,
            IMapper mapper) 
            : base(repository, emailSender, currentUserService, userManager)
        {
            _contestRegistrationService = contestRegistrationService;
            _mapper = mapper;
        }

        public override async Task HandleAsync(CreateIndividualContestRegistrationCommand command)
        {
            var contest = await Repository.FindAsync<Contest>(command.ViewModel.ContestId);
            if (contest == null) throw new EntityNotFoundException();
            
            var validationResult = await _contestRegistrationService.ValidateCreateIndividualContestRegistrationAsync(command.ViewModel);
            if (validationResult.Any()) throw new ValidationException(validationResult);
            
            var registration = new IndividualContestRegistration();

            _mapper.Map(command.ViewModel, registration);

            await FinishRegistrationAsync(command.ViewModel, registration, contest);
        }
    }
}
