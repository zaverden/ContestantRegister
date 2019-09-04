using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Features.Frontend.Home.Queries;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Features.Frontend.Home.QueryHandlers
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
