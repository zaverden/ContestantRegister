using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Controllers._Common.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Users.ViewModels;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Infrastructure.Filter.Attributes;
using ContestantRegister.Infrastructure.Filter.Contervers;
using ContestantRegister.Models;
using ContestantRegister.Utils.Filter;

namespace ContestantRegister.Cqrs.Features.Frontend.Users.Queries
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
