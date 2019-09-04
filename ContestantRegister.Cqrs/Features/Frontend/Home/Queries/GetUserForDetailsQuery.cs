using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Home.Queries
{
    public class GetUserForDetailsQuery : IQuery<ApplicationUser>
    {
        public string Id { get; set; }
    }
}
