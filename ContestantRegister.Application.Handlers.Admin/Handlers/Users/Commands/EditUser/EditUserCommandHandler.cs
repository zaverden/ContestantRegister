using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices;

namespace ContestantRegister.Cqrs.Features.Admin.Users.CommandHandlers
{
    internal class EditUserCommandHandler : EditMappedEntityCommandHandler<ApplicationUser, EditUserViewModel, string>
    {
        //private readonly IUserService _userService;

        public EditUserCommandHandler(IRepository repository, IMapper mapper/*, IUserService userService*/) : base(repository, mapper)
        {
            //_userService = userService;
        }

        //protected override async Task<List<KeyValuePair<string, string>>> ValidateViewModel(EditUserViewModel viewModel)
        //{
        //    return await _userService.ValidateUserAsync(viewModel);
        //}
    }
}
