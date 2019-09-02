﻿using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.Regions
{
    public class RegionViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string Name { get; set; }
    }
}