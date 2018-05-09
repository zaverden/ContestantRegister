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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace ContestantRegister
{
    [Authorize(Roles = Roles.Admin)]
    public class ContestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ContestsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Contests
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contests.OrderByDescending(item => item.Id).ToListAsync());
        }

        // GET: Contests/Create
        public async Task<IActionResult> Create()
        {
            await FillViewData();

            return View();
        }

        private async Task FillViewData(Contest contest = null)
        {
            if (contest?.CompClasses == null)
            {
                ViewData["CompClasses"] = new MultiSelectList(await _context.CompClasses.ToListAsync(), "Id", "Name");
            }
            else
            {
                ViewData["CompClasses"] = new MultiSelectList(await _context.CompClasses.ToListAsync(), "Id", "Name", contest.CompClasses.Select(c => c.CompClassId));
            }
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
                if (contest.SelectedCompClassIds != null)
                {
                    contest.CompClasses = await _context.CompClasses
                        .Where(c => contest.SelectedCompClassIds.Contains(c.Id))
                        .Select(c => new ContestCompClass {CompClass = c, CompClassId = c.Id, Contest = contest})
                        .ToListAsync();
                }
                _context.Add(contest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await FillViewData(contest);

            return View(contest);
        }

        //TODO зчем везде nullable id? Или пофиг, не править же генеренный код, который работает
        // GET: Contests/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contest = await _context.Contests
                .Include(c => c.CompClasses)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            await FillViewData(contest);

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
                    List<CompClass> selectedCompClasses = new List<CompClass>();
                    if (contest.SelectedCompClassIds != null)
                    {
                        selectedCompClasses = await _context.CompClasses
                            .Where(c => contest.SelectedCompClassIds.Contains(c.Id))
                            .ToListAsync();
                    }

                    var dbContest = await _context.Contests
                        .Include(c => c.CompClasses)
                        .Where(c => c.Id == id)
                        .SingleOrDefaultAsync();
                    if (dbContest.CompClasses == null)
                    {
                        dbContest.CompClasses = new List<ContestCompClass>();
                    }
                    foreach (var oldCompClass in dbContest.CompClasses.ToList())
                    {
                        if (!selectedCompClasses.Any(c => c.Id == oldCompClass.CompClassId))
                        {
                            dbContest.CompClasses.Remove(oldCompClass);
                        }
                    }
                    foreach (var compClass in selectedCompClasses)
                    {
                        if (!dbContest.CompClasses.Any(c => c.CompClassId == compClass.Id))
                        {
                            dbContest.CompClasses.Add(new ContestCompClass { CompClass = compClass, Contest = contest });
                        }
                    }

                    _mapper.Map(contest, dbContest);

                    _context.Update(dbContest);
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

            await FillViewData(contest);

            return View(contest);
        }

        private static void RemoveWindowsLineEnds(Contest contest)
        {
            if (!string.IsNullOrEmpty(contest.YaContestAccountsCSV) && contest.YaContestAccountsCSV.Contains('\r'))
            {
                contest.YaContestAccountsCSV = contest.YaContestAccountsCSV.Replace("\r", "");
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
