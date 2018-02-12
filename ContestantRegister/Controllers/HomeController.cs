using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ContestantRegister.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace ContestantRegister.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var actualContests = _context.Contests.Where(c => !c.IsArchive);

            return View(await actualContests.ToListAsync());
        }

        public async Task<IActionResult> Details(int id) //TODO как переименовать парамерт в contestId? Какой-то маппинг надо подставить
        {
            var contest = await _context.Contests.Include(c => c.ContestRegistrations)
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

            var registration = new IndividualContestRegistration
            {
                ContestId = contest.Id,
                Contest = contest,
            };

            var trainer = await _context.Users.OfType<Trainer>().Include(u => u.StudyPlace).SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var pupil = await _context.Users.OfType<Pupil>().Include(u => u.StudyPlace).SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var student = await _context.Users.OfType<Student>().Include(u => u.StudyPlace).SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

            ContestantUser user = null;
            if (trainer != null) user = trainer;
            if (pupil != null) user = pupil;
            if (student != null) user = student;

            if (contest.ParticipantType == ParticipantType.Pupil)
            {
                if (pupil != null)
                {
                    registration.Participant1 = pupil;
                }

                if (student != null)
                {
                    registration.Trainer = student;
                }

                if (trainer != null)
                {
                    registration.Trainer = trainer;
                }
                
            }

            if (contest.ParticipantType == ParticipantType.Student)
            {
                if (pupil != null)
                {
                    //TODO школота не участвуетв студ. соревнованиях. А вообще школьник не должен мочь на UI регаться на контест
                }

                if (student != null)
                {
                    registration.Participant1 = student;
                }

                if (trainer != null)
                {
                    registration.Trainer = trainer;
                }
            }
            

            return View(registration);
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
