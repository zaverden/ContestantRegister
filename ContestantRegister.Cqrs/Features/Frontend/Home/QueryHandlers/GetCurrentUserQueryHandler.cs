using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.QueryHandlers
{
    public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> HandleAsync(GetCurrentUserQuery query)
        {
            return await _userManager.FindByEmailAsync(query.CurrentUserEmail);
        }
    }
}
