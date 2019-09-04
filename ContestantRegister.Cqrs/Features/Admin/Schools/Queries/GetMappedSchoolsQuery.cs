using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Schools.ViewModels;
using ContestantRegister.Framework.Filter;
using ContestantRegister.Framework.Filter.Attributes;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Schools.Queries
{
    public class GetMappedSchoolsQuery : GetMappedEntitiesQuery<School, SchoolListItemViewModel>
    {
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string ShortName { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        //[RelatedObject("City", PropertyName = "Name")]
        public string City { get; set; }
    }    
}
