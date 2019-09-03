using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels
{
    public class UserListItemViewModel
    {
        [OrderBy]
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType UserType { get; set; }
        public string City { get; set; }
        public string StudyPlace { get; set; }

    }
}
