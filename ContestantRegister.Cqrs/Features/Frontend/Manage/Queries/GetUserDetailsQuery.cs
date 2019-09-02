using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.ViewModels.ManageViewModels;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Queries
{
    public class GetUserDetailsQuery : IQuery<IndexViewModel>
    {
        //TODO заменить на User.Identity.Name, там будет email
        public ClaimsPrincipal CurrentUser { get; set; }
    }
}
