using ContestantRegister.Cqrs.Features._Common.ListViewModel;
using ContestantRegister.Cqrs.Features._Common.Queries;
using ContestantRegister.Cqrs.Features.Admin.Users.ViewModels;
using ContestantRegister.Framework.Filter;
using ContestantRegister.Framework.Filter.Attributes;
using ContestantRegister.Framework.Filter.Contervers;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Users.Queries
{
    public class GetUsersQuery : GetMappedEntitiesQuery<ApplicationUser, UserListItemViewModel>
    {
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Email { get; set; }

        [ConvertFilter(typeof(NullableIntToNullableBooleanConverter))]
        public int? EmailConfirmed { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Surname { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Name { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        //[RelatedObject("StudyPlace.City", PropertyName = "Name")]
        public string City { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        //[RelatedObject("StudyPlace", PropertyName = "ShortName")]        
        public string StudyPlace { get; set; }

        [ConvertFilter(typeof(EnumDisplayToValueConverter<UserType>))]
        [PropertyName("UserType")]
        public string UserTypeName { get; set; }
    }
}
