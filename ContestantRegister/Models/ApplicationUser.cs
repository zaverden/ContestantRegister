using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ContestantUser> RegistredByThisUser { get; set; }
    }
}
