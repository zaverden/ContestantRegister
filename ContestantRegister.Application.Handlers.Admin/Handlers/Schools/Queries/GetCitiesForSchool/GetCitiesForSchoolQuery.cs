using System.Collections.Generic;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Schools.Queries
{
    public class GetCitiesForSchoolQuery : IQuery<List<City>>
    {
    }

}
