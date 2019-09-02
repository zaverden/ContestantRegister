using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Controllers;
using ContestantRegister.Data;
using ContestantRegister.Features.Frontend.Home.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContestantRegister.Features.Frontend.Home.CommansHandlers
{
    public class RegisterContestParticipantCommandHandler : ICommandHandler<RegisterContestParticipantCommand>
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper _urlHelper;
        public RegisterContestParticipantCommandHandler(IUserService userService, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailSender emailSender, ApplicationDbContext context, IUrlHelper urlHelper)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
        }

        public async Task HandleAsync(RegisterContestParticipantCommand command)
        {
            var validationResult = await _userService.ValidateUserAsync(command.RegisterContestParticipantViewModel);
            if (validationResult.Count != 0)
                throw new ValidationException(validationResult);

            var user = new ApplicationUser
            {
                UserName = command.RegisterContestParticipantViewModel.Email,
                RegistrationDateTime = DateTimeExtensions.SfuServerNow,
                RegistredBy = await _userManager.GetUserAsync(command.CurrentUser)
            };

            _mapper.Map(command.RegisterContestParticipantViewModel, user);

            var result = await _userManager.CreateAsync(user, command.RegisterContestParticipantViewModel.Email);
            if (!result.Succeeded)
                throw new UnableToCreateUserException(result.Errors);
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.EmailConfirmationLink(command.Action, command.Controller, user.Id, code, command.Scheme);
            await _emailSender.SendEmailConfirmationAsync(command.RegisterContestParticipantViewModel.Email, callbackUrl);
        }
    }
}
