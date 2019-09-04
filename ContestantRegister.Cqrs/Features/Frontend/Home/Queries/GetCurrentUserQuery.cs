using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Queries
{
    public class GetCurrentUserQuery : IQuery<ApplicationUser>
    {
        public string CurrentUserEmail { get; set; }
    }
}
