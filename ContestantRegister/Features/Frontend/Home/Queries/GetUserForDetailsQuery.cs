using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Features.Frontend.Home.Queries
{
    public class GetUserForDetailsQuery : IQuery<ApplicationUser>
    {
        public string Id { get; set; }
    }
}
