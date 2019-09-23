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
    public class EditIndividualContestRegistrationCommandHandler : RepositoryCommandBaseHandler<EditIndividualContestRegistrationCommand>
    {
        private readonly IContestRegistrationService _contestRegistrationService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public EditIndividualContestRegistrationCommandHandler(
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

        public override async Task HandleAsync(EditIndividualContestRegistrationCommand command)
        {
            var dbRegistration = await Repository.Set<IndividualContestRegistration>()
                .SingleOrDefaultAsync(r => r.Id == command.RegistrationId);
            if (dbRegistration == null) throw new EntityNotFoundException();
            

            var validationResult = await _contestRegistrationService.ValidateEditIndividualContestRegistrationAsync(command.ViewModel);
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
    }
}
