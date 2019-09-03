using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Controllers.Account.Commands;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContestantRegister.Controllers.Account.CommandHandlers
{
    public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterCommandHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        //TODO Для чистоты архитектуры можно вынести все команды, запросы и хендлеры в отдельную сборку, чтобы туда не просочилось что-то из AspNetMsc 
        //TODO Тогда вместо IUrlHelper нужно будет использовать свой интерфейс, а его реализацию делать вышестоящему слою
        private readonly IUrlHelper _urlHelper;

        public RegisterCommandHandler(IUserService userService, IMapper mapper, ILogger<RegisterCommandHandler> logger, IEmailSender emailSender, UserManager<ApplicationUser> userManager, IUrlHelper urlHelper)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
            _urlHelper = urlHelper;
        }

        public async Task HandleAsync(RegisterCommand command)
        {
            var validationResult = await _userService.ValidateUserAsync(command.RegisterViewModel);
            if (validationResult.Count > 0) throw new ValidationException(validationResult);

            var user = new ApplicationUser
            {
                UserName = command.RegisterViewModel.Email,
                RegistrationDateTime = DateTimeExtensions.SfuServerNow
            };

            _mapper.Map(command.RegisterViewModel, user);

            var result = await _userManager.CreateAsync(user, command.RegisterViewModel.PasswordViewModel.Password);
            if (!result.Succeeded)
                throw new UnableToCreateUserException(result.Errors);

            _logger.LogInformation("User created a new account with password.");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.Action(command.Action, command.Controller, new [] { user.Id, code}, command.RequestScheme);
            await _emailSender.SendEmailConfirmationAsync(command.RegisterViewModel.Email, callbackUrl);                            
        }
    }
}
