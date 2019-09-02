using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.Schools
{
    public class SchoolListItemViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string ShortName { get; set;}

        public string FullName { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string Site { get; set; }
    }
}
