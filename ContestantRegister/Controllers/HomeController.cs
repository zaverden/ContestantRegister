using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ContestantRegister.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using AutoMapper;
using ContestantRegister.Options;
using ContestantRegister.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContestantRegister.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ContestantRegister.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContestRegistrationService _contestRegistrationService;
        private readonly MailOptions _options;

        public HomeController(ILogger<HomeController> logger, 
            ApplicationDbContext context, 
            IMapper mapper,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IOptions<MailOptions> options,
            IContestRegistrationService contestRegistrationService)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
            _contestRegistrationService = contestRegistrationService;
            _options = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            var actualContests = _context.Contests.Where(c => !c.IsArchive);

            return View(await actualContests.ToListAsync());
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
        public async Task<IActionResult> Register(int id) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            if (contest.ContestType == ContestType.Collegiate) throw new NotImplementedException();

            var registration = new IndividualContestRegistrationViewModel
            {
                ContestName = contest.Name,
                ContestId = contest.Id,
            };

            var trainer = await _context.Users.OfType<Trainer>().Include(u => u.StudyPlace).SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var pupil = await _context.Users.OfType<Pupil>().Include(u => u.StudyPlace).SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var student = await _context.Users.OfType<Student>().Include(u => u.StudyPlace).SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

            ContestantUser user = null;
            if (trainer != null)
            {
                user = trainer;

                registration.TrainerId = user.Id;
            }

            if (pupil != null)
            {
                user = pupil;

                if (contest.ParticipantType == ParticipantType.Pupil)
                {
                    registration.Participant1Id = user.Id;
                }
                else
                    throw new Exception("Школьники не участвуют в студенческих соревнованиях");
            }

            if (student != null)
            {
                user = student;

                if (contest.ParticipantType == ParticipantType.Pupil)
                {
                    registration.TrainerId = user.Id;
                }
                else
                {
                    registration.Participant1Id = user.Id;
                }
            }

            if (contest.ParticipantType == ParticipantType.Pupil && user.StudyPlace is School ||
                contest.ParticipantType == ParticipantType.Student && user.StudyPlace is Institution)
            {
                registration.StudyPlaceId = user.StudyPlaceId;
                registration.CityId = user.StudyPlace.CityId;
            }

            ViewData["Participant1Id"] = new SelectList(_context.Users, "Id", "UserName", registration.Participant1Id);
            ViewData["TrainerId"] = new SelectList(_context.Users, "Id", "UserName", registration.TrainerId);
            ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName", registration.ManagerId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", registration.CityId);
            ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", registration.StudyPlaceId);

            if (contest.IsAreaRequired)
            {
                ViewData["Area"] = new SelectList(contest.Areas.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
            }

            return View(registration);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Register(int id, IndividualContestRegistrationViewModel viewModel)
        {
            var contest = await _context.Contests
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            var validationResult = await _contestRegistrationService.ValidateContestRegistrationAsync(viewModel, User, false);
            if (validationResult.Any())
            {
                validationResult.ForEach(res => ModelState.AddModelError(res.Key, res.Value));

                ViewData["Participant1Id"] = new SelectList(_context.Users, "Id", "UserName", viewModel.Participant1Id);
                ViewData["TrainerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.TrainerId);
                ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.ManagerId);
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);
                ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);

                if (contest.IsAreaRequired)
                {
                    ViewData["Area"] = new SelectList(contest.Areas.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries), viewModel.Area);
                }

                return View(viewModel);
            }

            var registration = new IndividualContestRegistration();

            _mapper.Map(viewModel, registration);
            registration.ContestId = contest.Id;
            registration.RegistrationDateTime = DateTime.Now;
            registration.RegistredBy = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.UserName == User.Identity.Name);
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
            await _context.SaveChangesAsync();

            //TODO Если регистрирует админ, то email не отправляется?
            if (contest.SendRegistrationEmail)
            {
                var participant = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.Id == viewModel.Participant1Id);
                await _emailSender.SendEmailAsync(participant.Email, 
                    "Вы зарегистрированы на контест", 
                    $"Вы успешно зарегистрированы на контест {contest.Name}. Ваши учетные данные для входа в систему: логин {registration.YaContestLogin} пароль {registration.YaContestPassword}{Environment.NewLine}. Ссылка для входа: {contest.YaContestLink} ");
            }
            
            return RedirectToAction(nameof(Details), new {id});
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
                .SingleOrDefaultAsync(r => r.Id == id);

            if (registration == null)
            {
                return NotFound();
            }

            var viewModel = new IndividualContestRegistrationViewModel
            {
                ContestName = registration.Contest.Name
            };
            _mapper.Map(registration, viewModel);

            ViewData["Participant1Id"] = new SelectList(_context.Users, "Id", "UserName", viewModel.Participant1Id);
            ViewData["TrainerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.TrainerId);
            ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.ManagerId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);
            ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);

            var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);
            if (contest.IsAreaRequired)
            {
                ViewData["Area"] = new SelectList(contest.Areas.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries), viewModel.Area);
            }

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
                
                ViewData["Participant1Id"] = new SelectList(_context.Users, "Id", "UserName", viewModel.Participant1Id);
                ViewData["TrainerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.TrainerId);
                ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.ManagerId);
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);
                ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);

                var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);
                if (contest.IsAreaRequired)
                {
                    ViewData["Area"] = new SelectList(contest.Areas.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries), viewModel.Area);
                }

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

            return RedirectToAction(nameof(Details), new {id = registration.ContestId});
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
            registration.RegistredBy = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.UserName == User.Identity.Name);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = registration.ContestId });
        }
    }
}
