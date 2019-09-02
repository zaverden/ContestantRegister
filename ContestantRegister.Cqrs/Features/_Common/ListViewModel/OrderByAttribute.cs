using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Models
{
    public class OrderByAttribute : Attribute
    {
        public bool IsDesc { get; set; }
    }
}
