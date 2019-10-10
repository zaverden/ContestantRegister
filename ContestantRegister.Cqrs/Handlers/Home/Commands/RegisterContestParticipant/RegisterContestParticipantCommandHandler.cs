using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features.Frontend.Home.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.CommansHandlers
{
    public class RegisterContestParticipantCommandHandler : ICommandHandler<RegisterContestParticipantCommand>
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper _urlHelper;
        private readonly ICurrentUserService _currentUserService;

        public RegisterContestParticipantCommandHandler(
            IUserService userService, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            IEmailSender emailSender, 
            IUrlHelper urlHelper,
            ICurrentUserService currentUserService)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
            _currentUserService = currentUserService;
        }

        public async Task HandleAsync(RegisterContestParticipantCommand command)
        {
            var validationResult = await _userService.ValidateUserAsync(command.RegisterContestParticipantViewModel);
            if (validationResult.Count != 0)
                throw new ValidationException(validationResult);

            var user = new ApplicationUser
            {
                UserName = command.RegisterContestParticipantViewModel.Email,
                RegistrationDateTime = DateTimeService.SfuServerNow,
                RegistredBy = await _userManager.FindByEmailAsync(_currentUserService.Email)
            };

            _mapper.Map(command.RegisterContestParticipantViewModel, user);

            var result = await _userManager.CreateAsync(user, command.RegisterContestParticipantViewModel.Email);
            if (!result.Succeeded)
                throw new UnableToCreateUserException(result.Errors);
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.Action(command.Action, command.Controller, new []{user.Id, code}, command.Scheme);
            await _emailSender.SendEmailConfirmationAsync(command.RegisterContestParticipantViewModel.Email, callbackUrl);
        }
    }
}
