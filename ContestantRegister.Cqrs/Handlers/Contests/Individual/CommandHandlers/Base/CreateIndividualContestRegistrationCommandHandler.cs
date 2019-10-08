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
    internal abstract class CreateIndividualContestRegistrationCommandHandler<TCommand> : CreateContestRegistrationCommandHandler<TCommand> 
             where TCommand : CreateIndividualContestRegistrationCommand
    {
        protected readonly IContestRegistrationService _contestRegistrationService;
        private readonly IMapper _mapper;

        protected CreateIndividualContestRegistrationCommandHandler(
            IRepository repository,
            IContestRegistrationService contestRegistrationService,
            IEmailSender emailSender, 
            ICurrentUserService currentUserService, 
            UserManager<ApplicationUser> userManager,
            IMapper mapper) 
            : base(repository, emailSender, currentUserService, userManager)
        {
            _contestRegistrationService = contestRegistrationService;
            _mapper = mapper;
        }

        public override async Task HandleAsync(TCommand command)
        {
            var contest = await Repository.FindAsync<Contest>(command.ViewModel.ContestId);
            if (contest == null) throw new EntityNotFoundException();

            var validationResult = await ValidateCreateIndividualContestRegistrationAsync(command.ViewModel); 
            if (validationResult.Any()) throw new ValidationException(validationResult);
            
            var registration = new IndividualContestRegistration();

            _mapper.Map(command.ViewModel, registration);

            await FinishRegistrationAsync(command.ViewModel, registration, contest);
        }

        protected virtual Task<List<KeyValuePair<string, string>>> ValidateCreateIndividualContestRegistrationAsync(
            CreateIndividualContestRegistrationViewModel viewModel)
        {
            return _contestRegistrationService.ValidateCreateIndividualContestRegistrationAsync(viewModel);
        }
    }
}
