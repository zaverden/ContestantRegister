using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Emails.Queries;
using ContestantRegister.Domain;
using ContestantRegister.Features;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Emails.QueryHandlers
{
    public class GetEmailsQueryHandler : ReadRepositoryQueryHandler<GetEmailsQuery, List<Email>>
    {
        public GetEmailsQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<List<Email>> HandleAsync(GetEmailsQuery query)
        {
            var emails = await ReadRepository.Set<Email>()
                .AutoFilter(query)
                .OrderByDescending(x => x.Id)
                .Take(100)
                .ToListAsync();

            return emails;
        }
    }
}
