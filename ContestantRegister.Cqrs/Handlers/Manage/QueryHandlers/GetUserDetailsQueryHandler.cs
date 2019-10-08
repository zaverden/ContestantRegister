using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Cqrs.Features._Common.QueryHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Manage.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.QueryHandlers
{
    internal class GetUserDetailsQueryHandler : ReadRepositoryQueryHandler<GetUserDetailsQuery, IndexViewModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetUserDetailsQueryHandler(IReadRepository repository, UserManager<ApplicationUser> userManager, IMapper mapper, ICurrentUserService currentUserService) : base(repository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public override async Task<IndexViewModel> HandleAsync(GetUserDetailsQuery query)
        {
            var user = await _userManager.FindByEmailAsync(_currentUserService.Email);
            if (user == null)
            {
                throw new EntityNotFoundException($"Unable to load user with ID '{_currentUserService.Email}'.");
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
