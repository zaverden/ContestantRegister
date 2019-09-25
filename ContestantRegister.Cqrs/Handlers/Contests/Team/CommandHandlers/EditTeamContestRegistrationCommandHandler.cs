using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Team.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices.ContestRegistration;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Team.CommandHandlers
{
    public class EditTeamContestRegistrationCommandHandler : RepositoryCommandBaseHandler<EditTeamContestRegistrationCommand>
    {
        private readonly IContestRegistrationService _contestRegistrationService;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditTeamContestRegistrationCommandHandler(
            IRepository repository, 
            IContestRegistrationService contestRegistrationService, 
            IMapper mapper, 
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager) : base(repository)
        {
            _contestRegistrationService = contestRegistrationService;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public override async Task HandleAsync(EditTeamContestRegistrationCommand command)
        {
            var dbRegistration = await Repository.Set<TeamContestRegistration>()
                .SingleOrDefaultAsync(r => r.Id == command.RegistrationId);
            if (dbRegistration == null)
            {
                throw new EntityNotFoundException();
            }

            var validationResult = await _contestRegistrationService.ValidateEditTeamContestRegistrationAsync(command.ViewModel);
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
