using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Authorization;
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
            ViewData["Areas"] = new MultiSelectList(await _context.Areas.ToListAsync(), "Id", "Name", contest?.ContestAreas.Select(c => c.AreaId));
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

                await SyncManyToMany(contest.SelectedAreaIds, _context.Areas, contest.ContestAreas, CreateContestAreaRelation, CmpArea, contest);

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
                .Include(c => c.ContestAreas)
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
                    var dbContest = await _context.Contests
                        .Include(c => c.ContestAreas)
                        .SingleOrDefaultAsync(c => c.Id == id);

                    await SyncManyToMany(contest.SelectedAreaIds, _context.Areas, dbContest.ContestAreas, CreateContestAreaRelation, CmpArea, dbContest);

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

        private bool CmpArea(Area area, ContestArea contestArea)
        {
            return area.Id == contestArea.AreaId;
        }

        private ContestArea CreateContestAreaRelation(Area area, Contest contest)
        {
            return new ContestArea
            {
                Area = area,
                Contest = contest,
            };
        }

        private async Task SyncManyToMany<TEntity, TRelationEntity>(int[] selectedIds, DbSet<TEntity> dbSet, ICollection<TRelationEntity> items, Func<TEntity, Contest, TRelationEntity> relationFactory, Func<TEntity, TRelationEntity, bool> comparator, Contest contest)
            where TEntity : DomainObject
            where TRelationEntity : DomainObject
        {
            var selectedItems = new List<TEntity>();
            if (selectedIds != null)
            {
                selectedItems = await dbSet.Where(item => selectedIds.Contains(item.Id)).ToListAsync();
            }

            foreach (var currentItem in items.ToList())
            {
                if (!selectedItems.Any(item => comparator(item, currentItem)))
                {
                    items.Remove(currentItem);
                }
            }

            foreach (var selectedItem in selectedItems)
            {
                if (!items.Any(item => comparator(selectedItem, item)))
                {
                    items.Add(relationFactory(selectedItem, contest));
                }
            }
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
