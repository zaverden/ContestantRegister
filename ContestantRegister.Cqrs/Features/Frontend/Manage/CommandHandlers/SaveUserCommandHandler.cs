using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Controllers._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Commands;
using ContestantRegister.Domain;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.CommandHandlers
{
    public class SaveUserCommandHandler : RepositoryCommandBaseHandler<SaveUserCommand>
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;


        public SaveUserCommandHandler(IRepository repository, IUserService userService, UserManager<ApplicationUser> userManager, IMapper mapper) : base(repository)
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public override async Task HandleAsync(SaveUserCommand command)
        {
            var validationResult = await _userService.ValidateUserAsync(command.ViewModel);
            if (validationResult.Count != 0) 
                throw new ValidationException(validationResult);

            
            var user = await _userManager.FindByEmailAsync(command.CurrentUserEmail);
            if (user == null)
            {
                throw new EntityNotFoundException($"Unable to load user with email '{command.CurrentUserEmail}'.");
            }

            _mapper.Map(command.ViewModel, user);

            Repository.Update(user);
            await Repository.SaveChangesAsync();
        }
    }
}
