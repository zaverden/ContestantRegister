using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.QueryHandlers
{
    public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<ApplicationUser> HandleAsync(GetCurrentUserQuery query)
        {
            return await _userManager.FindByEmailAsync(_currentUserService.Email);
        }
    }
}
