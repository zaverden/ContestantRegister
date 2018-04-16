using System;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.ContestViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContestantRegister
{
    [Authorize(Roles = Roles.Admin)]
    public class ContestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contests
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contests.ToListAsync());
        }

        // GET: Contests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contest contest)
        {
            if (ModelState.IsValid)
            {
                //Если создавать контест на винде, перевод строки там два символа. А потом при регистрации на никсах идет сплит по переводу строки, а это один символ. И \r добавляется сзади к паролю.
                //Это ломает экспорт в csv и при отправле пароля по email этот символ рендерится как пробел
                RemoveWindowsLineEnds(contest);

                _context.Add(contest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contest);
        }

        //TODO зчем везде nullable id? Или пофиг, не править же генеренный код, который работает
        // GET: Contests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var contest = await _context.Contests.SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }
            return View(contest);
        }

        // POST: Contests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contest contest)
        {
            if (id != contest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    RemoveWindowsLineEnds(contest);

                    //TODO Нужен ли Automapper?
                    _context.Update(contest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestExists(contest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contest);
        }

        private static void RemoveWindowsLineEnds(Contest contest)
        {
            if (!string.IsNullOrEmpty(contest.YaContestAccountsCSV) && contest.YaContestAccountsCSV.Contains('\r'))
            {
                contest.YaContestAccountsCSV = contest.YaContestAccountsCSV.Replace("\r", "");
            }

            if (!string.IsNullOrEmpty(contest.Areas) && contest.Areas.Contains('\r'))
            {
                contest.Areas = contest.Areas.Replace("\r", "");
            }
        }

        // GET: Contests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            return View(contest);
        }

        // POST: Contests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(m => m.Id == id);
            _context.Contests.Remove(contest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContestExists(int id)
        {
            return _context.Contests.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ImportParticipants(int id)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(contest.YaContestAccountsCSV))
            {
                throw new Exception("В контесте нет логинов-паролей для участников");
            }

            var viewModel = new ImportContestParticipantsViewModel();

            var contests = _context.Contests.Where(c => c.ParticipantType == contest.ParticipantType &&
                                                        c.ContestType == contest.ContestType &&
                                                        c.Id != contest.Id);
            ViewData["FromContestId"] = new SelectList(contests, "Id", "Name");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ImportParticipants(int id, ImportContestParticipantsViewModel viewModel)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(c => c.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var loginsForImport = viewModel.ParticipantYaContestLogins
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Select(login => login.TrimEnd('\r'))
                    .ToHashSet();
                var accounts = contest.YaContestAccountsCSV.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var registrations = _context.ContestRegistrations.Where(r => r.ContestId == viewModel.FromContestId);
                foreach (var registration in registrations)
                {
                    if (loginsForImport.Contains(registration.YaContestLogin))
                    {
                        if (contest.UsedAccountsCount == accounts.Length)
                        {
                            ModelState.AddModelError(string.Empty, "В контесте, в который импортируются участники, не хватает яконтест аккаунтов для завершения импорта");
                            var contests1 = _context.Contests.Where(c => c.ParticipantType == contest.ParticipantType &&
                                                                        c.ContestType == contest.ContestType &&
                                                                        c.Id != contest.Id);
                            ViewData["FromContestId"] = new SelectList(contests1, "Id", "Name", viewModel.FromContestId);
                            return View(viewModel);
                        }

                        var account = accounts[contest.UsedAccountsCount].Split(',');
                        var newRegistration = new IndividualContestRegistration
                        {
                            Status = ContestRegistrationStatus.ConfirmParticipation,
                            ProgrammingLanguage = registration.ProgrammingLanguage,
                            Participant1Id = registration.Participant1Id,
                            TrainerId = registration.TrainerId,
                            ManagerId = registration.ManagerId,
                            StudyPlaceId = registration.StudyPlaceId,
                            ContestId = id,
                            YaContestLogin = account[0],
                            YaContestPassword = account[1].TrimEnd('\r').TrimEnd('\n'),
                            Number = contest.RegistrationsCount + 1,
                        };
                        contest.UsedAccountsCount++;
                        contest.RegistrationsCount++;
                        _context.ContestRegistrations.Add(newRegistration);
                    }
                }

                await _context.SaveChangesAsync();

                //TODO Сказать о том, что участники успешно импортированы

                return RedirectToAction(nameof(HomeController.Details), "Home", new {id});
            }

            var contests = _context.Contests.Where(c => c.ParticipantType == contest.ParticipantType &&
                                                        c.ContestType == contest.ContestType &&
                                                        c.Id != contest.Id);
            ViewData["FromContestId"] = new SelectList(contests, "Id", "Name", viewModel.FromContestId);
            return View(viewModel);
        }
    }
}
