using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Utils;
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
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContestRegistrationService _contestRegistrationService;
        private readonly IUserService _userService;
        private readonly MailOptions _options;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            IMapper mapper,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IOptions<MailOptions> options,
            IContestRegistrationService contestRegistrationService,
            IUserService userService)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
            _contestRegistrationService = contestRegistrationService;
            _userService = userService;
            _options = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            var actualContests = _context.Contests.Where(c => !c.IsArchive);

            return View(await actualContests.ToListAsync());
        }

        public async Task<IActionResult> Archive()
        {
            var archiveContests = _context.Contests.Where(c => c.IsArchive);

            return View(await archiveContests.ToListAsync());
        }

        public async Task<IActionResult> Details(int id) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests
                .Include(c => c.ContestRegistrations)
                .Include("ContestRegistrations.Participant1")
                .Include("ContestRegistrations.Trainer")
                .Include("ContestRegistrations.Manager")
                .Include("ContestRegistrations.StudyPlace")
                .Include("ContestRegistrations.StudyPlace.City")
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            ICollection<ContestRegistration> userRegistrations = new List<ContestRegistration>();
            if (User.Identity.IsAuthenticated)
            {
                userRegistrations = await _context.ContestRegistrations
                    .Where(r => r.ContestId == id &&
                                (r.Participant1Id == user.Id || r.TrainerId == user.Id || r.ManagerId == user.Id))
                    .ToListAsync();
            }

            var viewModel = new IndividualContestDetailsViewModel
            {
                Contest = contest,
                UseRegistrations = userRegistrations,
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contestantUser = await _context.Users
                .Include(u => u.StudyPlace)
                .Include(u => u.StudyPlace.City)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contestantUser == null)
            {
                return NotFound();
            }

            return View(contestantUser);
        }

        [Authorize]
        public async Task<IActionResult> Register(int id) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            if (contest.ContestType == ContestType.Collegiate) throw new NotImplementedException();

            var viewModel = new IndividualContestRegistrationViewModel
            {
                ContestName = contest.Name,
                ContestId = contest.Id,
                ParticipantType = contest.ParticipantType
            };

            var user = await _userManager.GetUserAsync(User);
            user.StudyPlace = _context.StudyPlaces.Find(user.StudyPlaceId);
            switch (user.UserType)
            {
                case UserType.Trainer:
                    viewModel.TrainerId = user.Id;
                    break;

                case UserType.Pupil:
                    if (contest.ParticipantType == ParticipantType.Pupil)
                    {
                        viewModel.Participant1Id = user.Id;
                    }
                    else
                        throw new Exception("Школьники не участвуют в студенческих соревнованиях");
                    break;

                case UserType.Student:
                    if (contest.ParticipantType == ParticipantType.Pupil)
                    {
                        viewModel.TrainerId = user.Id;
                    }
                    else
                    {
                        viewModel.Participant1Id = user.Id;
                    }
                    break;
            }

            if (contest.ParticipantType == ParticipantType.Pupil && user.StudyPlace is School ||
                contest.ParticipantType == ParticipantType.Student && user.StudyPlace is Institution)
            {
                viewModel.StudyPlaceId = user.StudyPlaceId;
                viewModel.CityId = user.StudyPlace.CityId;
            }

            await FillViewDataForIndividualContestRegistration(viewModel, contest);

            return View(viewModel);
        }

        private async Task<Contest> GetContest(int contestId)
        {
            return await _context.Contests.SingleOrDefaultAsync(c => c.Id == contestId);
        }

        private async Task FillViewDataForIndividualContestRegistration(IndividualContestRegistrationViewModel viewModel, Contest contest)
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);
            var users = await GetListItemsAsync<ApplicationUser, UserListItemViewModel>(_context, _mapper);

            if (viewModel.ParticipantType == ParticipantType.Pupil)
            {
                ViewData["Participant1Id"] = new SelectList(users.Where(u => u.UserType == UserType.Pupil), "Id", "DisplayName", viewModel.Participant1Id);
                ViewData["TrainerId"] = new SelectList(users, "Id", "DisplayName", viewModel.TrainerId);
                ViewData["ManagerId"] = new SelectList(users, "Id", "DisplayName", viewModel.ManagerId);
                ViewData["StudyPlaces"] = await GetListItemsAsync<School, StudyPlaceListItemViewModel>(_context, _mapper);
            }
            else
            {
                ViewData["Participant1Id"] = new SelectList(users.Where(u => u.UserType == UserType.Student), "Id", "DisplayName", viewModel.Participant1Id);
                ViewData["TrainerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.TrainerId);
                ViewData["ManagerId"] = new SelectList(users.Where(u => u.UserType != UserType.Pupil), "Id", "DisplayName", viewModel.TrainerId);
                ViewData["StudyPlaces"] = await GetListItemsAsync<Institution, StudyPlaceListItemViewModel>(_context, _mapper);
            }

            if (contest.IsAreaRequired)
            {
                ViewData["Area"] = new SelectList(contest.Areas.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(int id, IndividualContestRegistrationViewModel viewModel)
        {
            var contest = await GetContest(id);
            if (contest == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateContestRegistrationAsync(viewModel, User, false);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                await FillViewDataForIndividualContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            var registration = new IndividualContestRegistration();

            _mapper.Map(viewModel, registration);
            registration.RegistrationDateTime = DateTime.Now;
            registration.RegistredBy = await _userManager.GetUserAsync(User);
            registration.Status = ContestRegistrationStatus.Completed;

            var yacontestaccount = contest.YaContestAccountsCSV
                .Split(Environment.NewLine)
                .Skip(contest.UsedAccountsCount)
                .First()
                .Split(',');
            contest.UsedAccountsCount++;

            registration.YaContestLogin = yacontestaccount[0];
            registration.YaContestPassword = yacontestaccount[1];

            _context.ContestRegistrations.Add(registration);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            //TODO Если регистрирует админ, то email не отправляется?
            if (contest.SendRegistrationEmail)
            {
                var participant = await _context.Users.SingleAsync(u => u.Id == viewModel.Participant1Id);
                await _emailSender.SendEmailAsync(participant.Email,
                    "Вы зарегистрированы на контест",
                    $"Вы успешно зарегистрированы на контест {contest.Name}. Ваши учетные данные для входа в систему: логин {registration.YaContestLogin} пароль {registration.YaContestPassword}{Environment.NewLine}. Ссылка для входа: {contest.YaContestLink} ");
            }

            return RedirectToAction(nameof(Details), new { id });
        }

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


            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["StudyPlaces"] = await GetListItemsAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);

            return View(vm);
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
                    RegistrationDateTime = DateTime.Now,
                    RegistredBy = await _userManager.GetUserAsync(User)
                };

                _mapper.Map(viewModel, user);

                var result = await _userManager.CreateAsync(user, viewModel.Email);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(viewModel.Email, callbackUrl);

                    return RedirectToAction(nameof(Register), new { id = viewModel.ContestId });
                }

                ModelState.AddErrors(result.Errors);
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);
            ViewData["StudyPlaces"] = await GetListItemsAsync<StudyPlace, StudyPlaceListItemViewModel>(_context, _mapper);

            return View(viewModel);
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
                $"Предложил {viewModel.Email}{Environment.NewLine}" +
                $"Город '{viewModel.City}'{Environment.NewLine}" +
                $"Краткое название '{viewModel.ShortName}'{Environment.NewLine}" +
                $"Полное название '{viewModel.FullName}'{Environment.NewLine}" +
                $"Сайт '{viewModel.Site}'{Environment.NewLine}" +
                $"Официальный email '{viewModel.SchoolEmail}'{Environment.NewLine}");

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
                $"Предложил {viewModel.Email}{Environment.NewLine}" +
                $"Город '{viewModel.City}'{Environment.NewLine}" +
                $"Краткое название '{viewModel.ShortName}'{Environment.NewLine}" +
                $"Полное название '{viewModel.FullName}'{Environment.NewLine}" +
                $"Краткое название англ '{viewModel.ShortNameEn}'{Environment.NewLine}" +
                $"Полное название англ '{viewModel.FullNameEn}'{Environment.NewLine}" +
                $"Сайт '{viewModel.Site}'{Environment.NewLine}");

            return RedirectToAction(nameof(StudyPlaceSuggested));
        }

        [Authorize]
        public async Task<IActionResult> EditRegistration(int id)
        {
            var registration = await _context.ContestRegistrations
                .OfType<IndividualContestRegistration>()
                .Include(r => r.Contest)
                .Include(r => r.StudyPlace)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (registration == null)
            {
                return NotFound();
            }

            var viewModel = new IndividualContestRegistrationViewModel
            {
                ContestName = registration.Contest.Name,
                RegistrationId = registration.Id,
                ParticipantType = registration.Contest.ParticipantType,
                CityId = registration.StudyPlace.CityId,
            };
            _mapper.Map(registration, viewModel);
            var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

            await FillViewDataForIndividualContestRegistration(viewModel, contest);

            return View(viewModel);
        }

        /// <summary>
        /// Метод не меняет состояние регистрации. Если было ожидание подтверждения, то ожидание и остается. Или лучше сразу подтверждать после изменения статуса?
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditRegistration(int id, IndividualContestRegistrationViewModel viewModel)
        {
            var dbRedistration = await _context.ContestRegistrations
                .OfType<IndividualContestRegistration>()
                .SingleOrDefaultAsync(r => r.Id == id);
            if (dbRedistration == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateContestRegistrationAsync(viewModel, User, true);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

                await FillViewDataForIndividualContestRegistration(viewModel, contest);

                return View(viewModel);
            }

            _mapper.Map(viewModel, dbRedistration);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dbRedistration.ContestId });
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> CancelRegistration(int id)
        {
            //TODO На UI переспросить "Вы точно уверены, что хотите отменить регистрацию?"

            var registration = await _context.ContestRegistrations.SingleOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            _context.ContestRegistrations.Remove(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }

        [Authorize]
        //TODO стоит ли делать POST вместо GET?
        public async Task<IActionResult> ConfirmParticipation(int id)
        {
            var registration = await _context.ContestRegistrations.SingleOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            registration.Status = ContestRegistrationStatus.Completed;
            registration.RegistrationDateTime = DateTime.Now;
            registration.RegistredBy = await _userManager.GetUserAsync(User);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }
    }
}
