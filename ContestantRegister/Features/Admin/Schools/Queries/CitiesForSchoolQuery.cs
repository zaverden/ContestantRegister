using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Controllers.Schools.Queries
{
    public class CitiesForSchoolQuery : IQuery<List<City>>
    {
    }

}
