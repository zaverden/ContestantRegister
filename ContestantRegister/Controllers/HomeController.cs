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
using System.Runtime.InteropServices.WindowsRuntime;
using AutoMapper;
using ContestantRegister.Data.Migrations;
using ContestantRegister.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContestantRegister.Services;

namespace ContestantRegister.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, 
            ApplicationDbContext context, 
            IMapper mapper,
            IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
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

            return View(contest);
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

            ViewData["Participant1Id"] = new SelectList(_context.Users, "Id", "UserName", viewModel.Participant1Id);
            ViewData["TrainerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.TrainerId);
            ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName", viewModel.ManagerId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", viewModel.CityId);
            ViewData["StudyPlaceId"] = new SelectList(_context.StudyPlaces, "Id", "ShortName", viewModel.StudyPlaceId);

            if (contest.IsProgrammingLanguageNeeded && string.IsNullOrEmpty(viewModel.ProgrammingLanguage))
                ModelState.AddModelError(nameof(viewModel.ProgrammingLanguage), "Поле \"Язык программирования\" обязательное");

            var studyPlace = await _context.StudyPlaces.OfType<StudyPlace>().SingleAsync(s => s.Id == viewModel.StudyPlaceId);
            if (contest.ParticipantType == ParticipantType.Pupil && studyPlace is Institution ||
                contest.ParticipantType == ParticipantType.Student && studyPlace is School)
                ModelState.AddModelError(nameof(viewModel.StudyPlaceId), "Тип учебного заведения не соответствует типу контеста");
            if (viewModel.CityId != studyPlace.CityId)
                ModelState.AddModelError(nameof(viewModel.CityId), "Выбранный город не соответствует городу учебного заведения");
            if (!ModelState.IsValid) return View(viewModel);

            var participanrRegistred = await _context.ContestRegistrations.AnyAsync(r => r.ContestId == id && r.Participant1Id == viewModel.Participant1Id);
            if (participanrRegistred) ModelState.AddModelError(nameof(viewModel.Participant1Id), "Указанный участник уже зарегистрирован в контесте");

            if (viewModel.Participant1Id == viewModel.TrainerId) ModelState.AddModelError(nameof(viewModel.TrainerId), "Участник не может быть своим тренером");
            if (viewModel.Participant1Id == viewModel.ManagerId) ModelState.AddModelError(nameof(viewModel.ManagerId), "Участник не может быть своим руководителем");
            
            var currentUser = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.UserName == User.Identity.Name);
            if (!User.IsInRole(Roles.Admin) && viewModel.Participant1Id != currentUser.Id && viewModel.TrainerId != currentUser.Id && viewModel.ManagerId != currentUser.Id)
                ModelState.AddModelError(string.Empty, "Вы должны быть участником, тренером или руководителем чтобы завершить регистрацию");

            if (!ModelState.IsValid) return View(viewModel);

            var participant = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.Id == viewModel.Participant1Id);
            var trainer = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.Id == viewModel.TrainerId);
            var manager = await _context.Users.OfType<ContestantUser>().SingleOrDefaultAsync(u => u.Id == viewModel.ManagerId);

            if (contest.ParticipantType == ParticipantType.Pupil)
            {
                if (!(participant is Pupil))
                {
                    ModelState.AddModelError(nameof(viewModel.Participant1Id), "Только школьник может быть участником школьного контеста");
                    return View(viewModel);
                }
            }

            if (contest.ParticipantType == ParticipantType.Student)
            {
                if (!(participant is Student)) ModelState.AddModelError(nameof(viewModel.Participant1Id), "Только студент может быть участником студенческого контеста");
                if (trainer is Pupil) ModelState.AddModelError(nameof(viewModel.TrainerId), "Школьник не может быть тренером на студенческом контесте");
                if (manager is Pupil) ModelState.AddModelError(nameof(viewModel.ManagerId), "Школьник не может быть руководителем на студенческом контесте");
            }

            if (!ModelState.IsValid) return View(viewModel);

            var registration = new IndividualContestRegistration();

            _mapper.Map(viewModel, registration);
            registration.ContestId = contest.Id;
            registration.RegistrationDateTime = DateTime.Now;
            registration.RegistredBy = currentUser;
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
                await _emailSender.SendEmailAsync(participant.Email, 
                    "Вы зарегистрированы на контест", 
                    $"Вы успешно зарегистрированы на контест {contest.Name}. Ваши учетные данные для входа в систему: логин {registration.YaContestLogin} пароль {registration.YaContestPassword} ");
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
    }
}
