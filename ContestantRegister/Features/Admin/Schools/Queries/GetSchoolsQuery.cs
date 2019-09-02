using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils.Filter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Infrastructure.Filter.Attributes;

namespace ContestantRegister.Controllers.Schools
{
    public class GetSchoolsQuery : GetEntitiesWithMappingQuery<School, SchoolListItemViewModel>
    {
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string ShortName { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        //[RelatedObject("City", PropertyName = "Name")]
        public string City { get; set; }
    }    
}
