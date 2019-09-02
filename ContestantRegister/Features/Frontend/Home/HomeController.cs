using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ContestantRegister.Application.Exceptions;
using ContestantRegister.Features.Frontend.Home.Commands;
using ContestantRegister.Features.Frontend.Home.Queries;
using ContestantRegister.Infrastructure.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.Contest;
using ContestantRegister.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ContestantRegister.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SuggestStudyPlaceOptions _options;
        private readonly IHandlerDispatcher _handlerDispatcher;
        
        public HomeController(
            ILogger<HomeController> logger,
            IOptions<SuggestStudyPlaceOptions> options,
            IHandlerDispatcher handlerDispatcher)
        {
            _logger = logger;
            _options = options.Value;
            _handlerDispatcher = handlerDispatcher;
        }

        public async Task<IActionResult> Index()
        {
            var actualContests = await _handlerDispatcher.ExecuteQueryAsync(new GetContestsForHomeQuery {IsArchived = false} );
            
            return View(actualContests);
        }

        public async Task<IActionResult> Archive()
        {
            var archiveContests = await _handlerDispatcher.ExecuteQueryAsync(new GetContestsForHomeQuery { IsArchived = true, IsOrderByDesc = true});
            
            return View(archiveContests);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            // Get the details of the exception that occurred
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                _logger.LogError(exceptionFeature.Error, $"Unhandled exception at {exceptionFeature.Path}");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region RegisterParticipant

        [Authorize]
        public IActionResult RegisterStudent(int id)
        {
            //TODO как передать несколько параметров с клиента? тогда лишний метод для навигации не нужен

            return RedirectToAction(nameof(RegisterContestParticipant), new { contestId = id, userType = UserType.Student });
        }

        [Authorize]
        public IActionResult RegisterPupil(int id)
        {
            //TODO как передать несколько параметров с клиента? тогда лишний метод для навигации не нужен

            return RedirectToAction(nameof(RegisterContestParticipant), new { contestId = id, userType = UserType.Pupil });
        }

        [Authorize]
        public IActionResult RegisterTrainer(int id)
        {
            //TODO как передать несколько параметров с клиента? тогда лишний метод для навигации не нужен

            return RedirectToAction(nameof(RegisterContestParticipant), new { contestId = id, userType = UserType.Trainer });
        }

        [Authorize]
        public async Task<IActionResult> RegisterContestParticipant(int contestId, UserType userType)
        {
            //TODO можно добавить отправку на клиент типа контеста индивидуальный или командный, чтобы не запрашивать его в POST методе
            var vm = new RegisterContestParticipantViewModel
            {
                ContestId = contestId,
                UserType = userType,
                IsUserTypeDisabled = true
            };

            await FillRegisterContestParticipantViewData();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegisterContestParticipant(RegisterContestParticipantViewModel viewModel)
        {
            try
            {
                await _handlerDispatcher.ExecuteCommandAsync(new RegisterContestParticipantCommand
                {
                    RegisterContestParticipantViewModel = viewModel,
                    CurrentUser = User,
                    Scheme = Request.Scheme,
                    Controller = "Account",
                    Action = nameof(AccountController.ConfirmEmail)
                });
            }
            catch (Exception ex) when (ex is ValidationException || ex is UnableToCreateUserException)
            {
                if (ex is ValidationException validException)
                    validException.ValidationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));
                if (ex is UnableToCreateUserException unableException)
                    ModelState.AddErrors(unableException.Errors);

                await FillRegisterContestParticipantViewData();

                return View(viewModel);
            }
            
            var contestType = await _handlerDispatcher.ExecuteQueryAsync(new GetContestTypeForHomeQuery {Id = viewModel.ContestId});
            return contestType == ContestType.Individual ? 
                     RedirectToAction(nameof(IndividualContestController.Register), "IndividualContest", new { id = viewModel.ContestId }) :
                     RedirectToAction(nameof(TeamContestController.Register), "TeamContest", new { id = viewModel.ContestId });
           }

        #endregion

        public async Task<IActionResult> SuggestSchool()
        {
            var viewModel = new SuggectSchoolViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _handlerDispatcher.ExecuteQueryAsync(new GetCurrentUserQuery {CurrentUser = User});
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestSchool(SuggectSchoolViewModel viewModel)
        {
            await _handlerDispatcher.ExecuteCommandAsync(new SendEmailCommand
            {
                Email = _options.Email,
                Subject = "Новая школа",
                Body = $"Предложил {viewModel.Email}<br>" +
                       $"Краткое название {viewModel.ShortName}<br>" +
                       $"Полное название {viewModel.FullName}<br>" +
                       $"Регион {viewModel.Region}<br>" +
                       $"Город {viewModel.City}<br>" +
                       $"Официальный email {viewModel.SchoolEmail}<br>" +
                       $"Сайт {viewModel.Site}<br>"
            });
            
            return RedirectToAction(nameof(StudyPlaceSuggested));
        }

        public IActionResult StudyPlaceSuggested()
        {
            return View();
        }

        public async Task<IActionResult> SuggestInstitution()
        {
            var viewModel = new SuggectInstitutionViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _handlerDispatcher.ExecuteQueryAsync(new GetCurrentUserQuery { CurrentUser = User });
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestInstitution(SuggectInstitutionViewModel viewModel)
        {
            await _handlerDispatcher.ExecuteCommandAsync(new SendEmailCommand
            {
                Email = _options.Email,
                Subject = "Новый вуз",
                Body = $"Предложил {viewModel.Email}<br>" +
                       $"Краткое название {viewModel.ShortName}<br>" +
                       $"Полное название {viewModel.FullName}<br>" +
                       $"Регион {viewModel.Region}<br>" +
                       $"Город {viewModel.City}<br>" +
                       $"Краткое название англ {viewModel.ShortNameEn}<br>" +
                       $"Полное название англ {viewModel.FullNameEn}<br>" +
                       $"Сайт {viewModel.Site}<br>"
            });

            return RedirectToAction(nameof(StudyPlaceSuggested));
        }

        //TODO Нужен ли вообще этот метод?
        [Authorize]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null) return NotFound();

            var contestantUser = await _handlerDispatcher.ExecuteQueryAsync(new GetUserForDetailsQuery {Id = id});

            if (contestantUser == null) return NotFound();
            
            return View(contestantUser);
        }

        private async Task FillRegisterContestParticipantViewData()
        {
            var data = await _handlerDispatcher.ExecuteQueryAsync(new GetRegisterParticipantDataQuery());

            ViewData["CityId"] = new SelectList(data.Cities, "Id", "Name");
            ViewData["StudyPlaces"] = data.StudyPlaces;
        }
    }
}
