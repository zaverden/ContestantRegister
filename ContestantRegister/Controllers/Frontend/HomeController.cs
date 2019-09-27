using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels;
using ContestantRegister.Application.Handlers.Frontend.Handlers.Home.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Home.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Home.Queries;
using ContestantRegister.Cqrs.Features.Frontend.Home.ViewModels;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContestantRegister.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHandlerDispatcher _handlerDispatcher;
        
        public HomeController(IHandlerDispatcher handlerDispatcher)
        {
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

            var viewModel = new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier};

            if (exceptionFeature != null)
            {
                if (exceptionFeature.Error is EntityNotFoundException)
                {
                    viewModel.Message = "Не удалось найти запрашиваемый объект";
                }
            }

            return View(viewModel);
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
                var user = await _handlerDispatcher.ExecuteQueryAsync(new GetCurrentUserQuery());
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestSchool(SuggectSchoolViewModel viewModel)
        {
            await _handlerDispatcher.ExecuteCommandAsync(new SuggestSchoolCommand { ViewModel = viewModel });
            
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
                var user = await _handlerDispatcher.ExecuteQueryAsync(new GetCurrentUserQuery ());
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestInstitution(SuggectInstitutionViewModel viewModel)
        {
            await _handlerDispatcher.ExecuteCommandAsync(new SuggestInstitutionCommand { ViewModel = viewModel });

            return RedirectToAction(nameof(StudyPlaceSuggested));
        }

        //TODO Нужен ли вообще этот метод?
        [Authorize]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null) throw new EntityNotFoundException();

            var contestantUser = await _handlerDispatcher.ExecuteQueryAsync(new GetUserForDetailsQuery {Id = id});

            if (contestantUser == null) throw new EntityNotFoundException();

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
