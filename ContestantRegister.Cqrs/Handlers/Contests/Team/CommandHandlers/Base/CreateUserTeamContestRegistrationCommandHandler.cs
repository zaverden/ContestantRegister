using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers;
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
    internal abstract class CreateTeamContestRegistrationCommandHandler<TCommand> : CreateContestRegistrationCommandHandler<TCommand>
        where TCommand : CreateTeamContestRegistrationCommand
    {
        private readonly IMapper _mapper;
        protected readonly IContestRegistrationService _contestRegistrationService;

        protected CreateTeamContestRegistrationCommandHandler(
            IRepository repository, 
            IMapper mapper, 
            IContestRegistrationService contestRegistrationService,
            IEmailSender emailSender,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager
            ) 
            : base(repository, emailSender, currentUserService, userManager)
        {
            _mapper = mapper;
            _contestRegistrationService = contestRegistrationService;
        }

        public override async Task HandleAsync(TCommand command)
        {
            var contest = await Repository.Set<Contest>()
                .SingleOrDefaultAsync(x => x.Id == command.ViewModel.ContestId);
            if (contest == null)
            {
                throw new EntityNotFoundException();
            }

            var validationResult = await ValidateCreateTeamContestRegistrationAsync(command.ViewModel);
            if (validationResult.Any())
            {
                throw new ValidationException(validationResult);
            }

            var registration = new TeamContestRegistration();

            _mapper.Map(command.ViewModel, registration);


            var registredForStudyPlaceCount = await Repository.Set<ContestRegistration>().CountAsync(r =>
                r.StudyPlaceId == registration.StudyPlaceId && r.ContestId == command.ViewModel.ContestId); 
            string studyPlaceName = string.Empty;
            var studyPlace = await Repository.FindAsync<StudyPlace>(registration.StudyPlaceId);

            if (contest.ParticipantType == ParticipantType.Student && contest.IsEnglishLanguage && studyPlace is Institution inst)
            {
                studyPlaceName = inst.ShortNameEnglish;
            }
            else
            {
                studyPlaceName = studyPlace.ShortName;
            }

            string sharp = contest.ShowSharpTeamNumber ? "#" : "";
            registration.OfficialTeamName = $"{studyPlaceName} {sharp}{registredForStudyPlaceCount + 1}";

            await FinishRegistrationAsync(command.ViewModel, registration, contest);
        }

        protected virtual Task<List<KeyValuePair<string, string>>> ValidateCreateTeamContestRegistrationAsync(
            CreateTeamContestRegistrationViewModel viewModel)
        {
            return _contestRegistrationService.ValidateCreateTeamContestRegistrationAsync(viewModel);
        }
    }
}
