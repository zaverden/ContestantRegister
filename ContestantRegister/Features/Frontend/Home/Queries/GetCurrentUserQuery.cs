using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Features.Frontend.Home.Queries
{
    public class GetCurrentUserQuery : IQuery<ApplicationUser>
    {
        public string CurrentUserEmail { get; set; }
    }
}
