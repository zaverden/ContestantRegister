﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Admin.Users.Queries;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Admin.Users.QueryHandlers
{
    internal class GetDataForUserDetailsQueryHandler : ReadRepositoryQueryHandler<GetDataForUserDetailsQuery, DataForUserDetails>
    {
        public GetDataForUserDetailsQueryHandler(IReadRepository repository) : base(repository)
        {
        }

        public override async Task<DataForUserDetails> HandleAsync(GetDataForUserDetailsQuery query)
        {
            var result = new DataForUserDetails();

            result.Cities = await ReadRepository.Set<City>()
                .OrderBy(x => x.Name)
                .ToListAsync();

            result.StudyPlaces = await ReadRepository.Set<StudyPlace>()
                .ProjectTo<StudyPlaceDropdownItemViewModel>()
                .ToListAsync();

            return result;
        }
    }
}
