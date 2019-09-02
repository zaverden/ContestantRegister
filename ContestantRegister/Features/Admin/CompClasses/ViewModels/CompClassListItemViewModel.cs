using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.CompClasses
{
    public class CompClassListItemViewModel 
    {
        public int Id { get; set; }
        [OrderBy]
        public string Name { get; set;}
        public string Area { get; set; }

        public string CompNumber { get; set; }
        public string Comment { get; set; }
    }
}
