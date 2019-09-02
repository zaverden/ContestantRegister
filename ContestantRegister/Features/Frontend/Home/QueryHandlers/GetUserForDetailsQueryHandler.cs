using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers._Common.QueryHandlers;
using ContestantRegister.Data;
using ContestantRegister.Domain;
using ContestantRegister.Features.Frontend.Home.Queries;
using ContestantRegister.Models;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Features.Frontend.Home.QueryHandlers
{
    public class GetUserForDetailsQueryHandler : ReadRepositoryQueryHandler<GetUserForDetailsQuery, ApplicationUser>
    {
        public GetUserForDetailsQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<ApplicationUser> HandleAsync(GetUserForDetailsQuery query)
        {
            return await  ReadRepository.Set<ApplicationUser>()
                .Include(u => u.StudyPlace.City)
                .SingleOrDefaultAsync(m => m.Id == query.Id);
        }
    }
}
