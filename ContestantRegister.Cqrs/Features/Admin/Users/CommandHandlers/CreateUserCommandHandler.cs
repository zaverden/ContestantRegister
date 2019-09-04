using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.Commands;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Admin.Users.CommandHandlers
{
    public class CreateUserCommandHandler : CreateMappedEntityCommandHandler<CreateUserCommand, ApplicationUser, CreateUserViewModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IRepository repository, IMapper mapper, UserManager<ApplicationUser> userManager, IUserService userService) : base(repository, mapper)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public override async Task HandleAsync(CreateUserCommand command)
        {
            var validationResult = await ValidateViewModel(command.Entity);
            if (validationResult?.Count > 0)
                throw new ValidationException(validationResult);

            var newUser = new ApplicationUser();
            await InitNewEntity(newUser, command);

            Mapper.Map(command.Entity, newUser);

            var result = await _userManager.CreateAsync(newUser, command.Entity.PasswordViewModel.Password);
            if (!result.Succeeded)
                throw new UnableToCreateUserException(result.Errors);
        }

        protected override async Task InitNewEntity(ApplicationUser entity, CreateUserCommand command)
        {
            entity.UserName = command.Entity.Email;
            entity.RegistrationDateTime = DateTimeService.SfuServerNow;
            entity.RegistredBy = await _userManager.FindByEmailAsync(command.CurrentUserEmail);
        }

        protected override async Task<List<KeyValuePair<string, string>>> ValidateViewModel(CreateUserViewModel viewModel)
        {
            return await _userService.ValidateUserAsync(viewModel);
        }
    }
}
