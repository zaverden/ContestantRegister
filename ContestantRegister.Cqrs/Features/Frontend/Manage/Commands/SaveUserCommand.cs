using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.ViewModels.ManageViewModels;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.Commands
{
    public class SaveUserCommand : ICommand
    {
        public IndexViewModel ViewModel { get; set; }

        public string CurrentUserEmail { get; set; }
    }
}
