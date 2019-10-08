using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices.ContestRegistration;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Individual.CommandHandlers
{
    internal abstract class EditIndividualContestRegistrationCommandHandler<TCommand> : RepositoryCommandBaseHandler<TCommand>
            where TCommand : EditIndividualContestRegistrationCommand
    {
        protected readonly IContestRegistrationService _contestRegistrationService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        protected readonly ICurrentUserService _currentUserService;

        protected EditIndividualContestRegistrationCommandHandler(
            IRepository repository, 
            IContestRegistrationService contestRegistrationService, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService
            ) 
            : base(repository)
        {
            _contestRegistrationService = contestRegistrationService;
            _mapper = mapper;
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public override async Task HandleAsync(TCommand command)
        {
            var dbRegistration = await Repository.Set<IndividualContestRegistration>()
                .SingleOrDefaultAsync(r => r.Id == command.RegistrationId);
            if (dbRegistration == null) throw new EntityNotFoundException();
            

            var validationResult = await ValidateEditIndividualContestRegistrationAsync(command.ViewModel);
            if (validationResult.Any())
            {
                throw new ValidationException(validationResult);
            }

            _mapper.Map(command.ViewModel, dbRegistration);

            if (dbRegistration.Status == ContestRegistrationStatus.Completed && dbRegistration.RegistrationDateTime == null)
            {
                dbRegistration.RegistrationDateTime = DateTimeService.SfuServerNow;
                dbRegistration.RegistredBy = await _userManager.FindByEmailAsync(_currentUserService.Email);
            }

            await Repository.SaveChangesAsync();
        }

        protected virtual async Task<List<KeyValuePair<string, string>>> ValidateEditIndividualContestRegistrationAsync(EditIndividualContestRegistrationViewModel viewModel)
        {
            return await _contestRegistrationService.ValidateEditIndividualContestRegistrationAsync(viewModel);
        }
    }
}
