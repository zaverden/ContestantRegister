using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.Institutions.ViewModels
{
    public class InstitutionListItemViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string ShortName { get; set; }

        public string FullName { get; set; }

        public string City { get; set; }

        public string ShortNameEnglish { get; set; }

        public string FullNameEnglish { get; set; }

        public string BaylorFullName { get; set; }

        public string Site { get; set; }
    }
}


