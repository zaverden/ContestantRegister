using ContestantRegister.Controllers;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            return View(await _context.Contests.OrderByDescending(item => item.Id).ToListAsync());
        }

        // GET: Contests/Create
        public IActionResult Create()
        {
            FillViewData();

            return View();
        }

        private void FillViewData()
        {
            ViewData["CompClasses"] = _context.CompClasses;
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

            FillViewData();

            return View(contest);
        }

        //TODO зчем везде nullable id? Или пофиг, не править же генеренный код, который работает
        // GET: Contests/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contest = await _context.Contests.SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            FillViewData();

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

            FillViewData();

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
    }
}
