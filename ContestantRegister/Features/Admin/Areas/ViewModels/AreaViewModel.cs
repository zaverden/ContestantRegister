using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.Areas.ViewModels
{
    public class AreaViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string Name { get; set; }
    }
}
