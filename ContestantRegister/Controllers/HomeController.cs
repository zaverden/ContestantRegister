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
using ContestantRegister.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContestantRegister.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
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
                Contest = contest,
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
                registration.Trainer = user;
            }

            if (pupil != null)
            {
                user = pupil;

                if (contest.ParticipantType == ParticipantType.Pupil)
                {
                    registration.Participant1Id = user.Id;
                    registration.Participant1 = user;
                }
                else
                    throw new Exception("Школота не участвуетв студ. соревнованиях");
            }

            if (student != null)
            {
                user = student;

                if (contest.ParticipantType == ParticipantType.Pupil)
                {
                    registration.TrainerId = user.Id;
                    registration.Trainer = user;
                }
                else
                {
                    registration.Participant1Id = user.Id;
                    registration.Participant1 = user;
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

            if (ModelState.IsValid)
            {
                var registration = new IndividualContestRegistration();

                _mapper.Map(viewModel, registration);
                registration.ContestId = contest.Id;
                registration.RegistrationDateTime = DateTime.Now;
                registration.RegistredBy = _context.Users.OfType<ContestantUser>().Single(u => u.UserName == User.Identity.Name);
                registration.Status = ContestRegistrationStatus.Completed;
                
                //TODO яконтест логин и пароль
                //TODO валидация что участник не регается два раза итд

                _context.ContestRegistrations.Add(registration);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new {id});
            }

            return View(viewModel);
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
