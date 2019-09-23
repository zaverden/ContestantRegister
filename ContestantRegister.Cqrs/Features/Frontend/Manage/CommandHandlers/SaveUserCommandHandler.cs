using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Commands;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.CommandHandlers
{
    public class SaveUserCommandHandler : RepositoryCommandBaseHandler<SaveUserCommand>
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;


        public SaveUserCommandHandler(IRepository repository, IUserService userService, UserManager<ApplicationUser> userManager, IMapper mapper, ICurrentUserService currentUserService) : base(repository)
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public override async Task HandleAsync(SaveUserCommand command)
        {
            var validationResult = await _userService.ValidateUserAsync(command.ViewModel);
            if (validationResult.Count != 0) 
                throw new ValidationException(validationResult);

            
            var user = await _userManager.FindByEmailAsync(_currentUserService.Email);
            if (user == null)
            {
                throw new EntityNotFoundException($"Unable to load user with email '{_currentUserService.Email}'.");
            }

            _mapper.Map(command.ViewModel, user);

            Repository.Update(user);
            await Repository.SaveChangesAsync();
        }
    }
}
