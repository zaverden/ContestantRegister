using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.QueryHandlers
{
    public class GetUserDetailsQueryHandler : ReadRepositoryQueryHandler<GetUserDetailsQuery, IndexViewModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public GetUserDetailsQueryHandler(IReadRepository repository, UserManager<ApplicationUser> userManager, IMapper mapper) : base(repository)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public override async Task<IndexViewModel> HandleAsync(GetUserDetailsQuery query)
        {
            var user = await _userManager.FindByEmailAsync(query.CurrentUserEmail);
            if (user == null)
            {
                throw new EntityNotFoundException($"Unable to load user with ID '{query.CurrentUserEmail}'.");
            }

            var studyPlace = await ReadRepository.Set<StudyPlace>().SingleAsync(sp => sp.Id == user.StudyPlaceId);

            var viewModel = new IndexViewModel
            {
                CanSuggestStudyPlace = true,
                CityId = studyPlace.CityId,
            };

            _mapper.Map(user, viewModel);

            return viewModel;
        }
    }
}
