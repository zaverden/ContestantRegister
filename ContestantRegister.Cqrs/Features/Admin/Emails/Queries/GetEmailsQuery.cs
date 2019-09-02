using System;
using System.Collections.Generic;
using System.Text;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Infrastructure.Filter.Attributes;
using ContestantRegister.Infrastructure.Filter.Contervers;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Emails.Queries
{
    public class GetEmailsQuery : IQuery<List<Email>>
    {
        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        [PropertyName("Address")]
        public string Email { get; set; }

        [ConvertFilter(typeof(NullableIntToNullableBooleanConverter))]
        [PropertyName("IsSended")]
        public int? Sended { get; set; }

        [StringFilter(StringFilter.Contains, IgnoreCase = true)]
        public string Message { get; set; }
    }
}
