using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.Contest;
using ContestantRegister.ViewModels.HomeViewModels;
using ContestantRegister.ViewModels.ListItemViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ContestantRegister.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SuggestStudyPlaceOptions _options;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            IMapper mapper,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IOptions<SuggestStudyPlaceOptions> options,
            IUserService userService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
            _options = options.Value;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var actualContests = _context.Contests.Where(c => !c.IsArchive);

            return View(await actualContests.OrderBy(item => item.Id).ToListAsync());
        }

        public async Task<IActionResult> Archive()
        {
            var archiveContests = _context.Contests.Where(c => c.IsArchive);

            return View(await archiveContests.OrderByDescending(item => item.Id).ToListAsync());
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
            var vm = new RegisterContestParticipantViewModel
            {
                ContestId = contestId,
                UserType = userType,
                IsUserTypeDisabled = true
            };

            await FillRegisterContestParticipantViewData();

            return View(vm);
        }

        private async Task FillRegisterContestParticipantViewData()
        {
            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name");
            var studyPlaces = await GetListItemsAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);
            ViewData["StudyPlaces"] = studyPlaces.OrderBy(s => s.ShortName);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegisterContestParticipant(RegisterContestParticipantViewModel viewModel)
        {
            var validationResult = await _userService.ValidateUserAsync(viewModel);
            validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = viewModel.Email,
                    RegistrationDateTime = Extensions.SfuServerNow,
                    RegistredBy = await _userManager.GetUserAsync(User)
                };

                _mapper.Map(viewModel, user);

                var result = await _userManager.CreateAsync(user, viewModel.Email);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(viewModel.Email, callbackUrl);
                    var contest = _context.Contests.Find(viewModel.ContestId);
                    if (contest.ContestType == ContestType.Individual)
                    {
                        return RedirectToAction(nameof(IndividualContestController.Register), "IndividualContest", new { id = viewModel.ContestId });
                    }
                    return RedirectToAction(nameof(TeamContestController.Register), "TeamContest", new { id = viewModel.ContestId });
                }

                ModelState.AddErrors(result.Errors);
            }

            await FillRegisterContestParticipantViewData();

            return View(viewModel);
        }

        #endregion

        public async Task<IActionResult> SuggestSchool()
        {
            var viewModel = new SuggectSchoolViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestSchool(SuggectSchoolViewModel viewModel)
        {
            await _emailSender.SendEmailAsync(_options.Email, "Новая школа",
                $"Предложил {viewModel.Email}<br>" +
                $"Краткое название {viewModel.ShortName}<br>" +
                $"Полное название {viewModel.FullName}<br>" +
                $"Регион {viewModel.Region}<br>" +
                $"Город {viewModel.City}<br>" +
                $"Официальный email {viewModel.SchoolEmail}<br>" +
                $"Сайт {viewModel.Site}<br>");

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
                var user = await _userManager.GetUserAsync(User);
                viewModel.Email = user.Email;
                viewModel.IsEmailReadonly = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SuggestInstitution(SuggectInstitutionViewModel viewModel)
        {
            await _emailSender.SendEmailAsync(_options.Email, "Новый вуз",
                $"Предложил {viewModel.Email}<br>" +
                $"Краткое название {viewModel.ShortName}<br>" +
                $"Полное название {viewModel.FullName}<br>" +
                $"Регион {viewModel.Region}<br>" +
                $"Город {viewModel.City}<br>" +
                $"Краткое название англ {viewModel.ShortNameEn}<br>" +
                $"Полное название англ {viewModel.FullNameEn}<br>" +
                $"Сайт {viewModel.Site}<br>");

            return RedirectToAction(nameof(StudyPlaceSuggested));
        }

        //TODO Нужен ли вообще этот метод?
        [Authorize]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contestantUser = await _context.Users
                .Include(u => u.StudyPlace.City)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (contestantUser == null)
            {
                return NotFound();
            }

            return View(contestantUser);
        }

        
    }
}
