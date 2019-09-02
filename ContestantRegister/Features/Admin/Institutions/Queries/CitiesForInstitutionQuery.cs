using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Controllers.Institutions.Queries
{
    public class CitiesForInstitutionQuery : IQuery<List<City>>
    {
    }
}
